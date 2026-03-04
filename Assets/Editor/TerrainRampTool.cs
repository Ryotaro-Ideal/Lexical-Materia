using UnityEngine;
using UnityEditor;

public class TerrainRampTool : EditorWindow
{
    private Terrain targetTerrain;
    private Transform startPoint;
    private Transform endPoint;
    private float rampWidth = 5.0f;
    private float falloff = 2.0f;

    // 追加機能用パラメータ
    private float offsetValue = 10.0f;
    private Axis offsetAxis = Axis.Z;

    private enum Axis { X, Z }

    [MenuItem("Tools/Terrain Ramp Tool")]
    public static void ShowWindow()
    {
        GetWindow<TerrainRampTool>("Terrain Ramp Tool");
    }

    private void OnEnable()
    {
        // 設定の復元
        rampWidth = EditorPrefs.GetFloat("TerrainRamp_Width", 5.0f);
        falloff = EditorPrefs.GetFloat("TerrainRamp_Falloff", 2.0f);
        offsetValue = EditorPrefs.GetFloat("TerrainRamp_Offset", 10.0f);
        offsetAxis = (Axis)EditorPrefs.GetInt("TerrainRamp_Axis", (int)Axis.Z);

        // オブジェクトの再検索 (シーン内のデフォルト名を検索)
        if (targetTerrain == null)
        {
            // まずActiveなものを探す
            if (Terrain.activeTerrain != null)
            {
                targetTerrain = Terrain.activeTerrain;
            }
            else
            {
                // 名前で保存されていたものを探す (非推奨だが補助として)
                string terrainName = EditorPrefs.GetString("TerrainRamp_TerrainName", "");
                if (!string.IsNullOrEmpty(terrainName))
                {
                    GameObject obj = GameObject.Find(terrainName);
                    if (obj != null) targetTerrain = obj.GetComponent<Terrain>();
                }
            }
        }

        if (startPoint == null)
        {
            GameObject startObj = GameObject.Find("Ramp_Start");
            if (startObj != null) startPoint = startObj.transform;
        }

        if (endPoint == null)
        {
            GameObject endObj = GameObject.Find("Ramp_End");
            if (endObj != null) endPoint = endObj.transform;
        }
    }

    private void OnDisable()
    {
        // 設定の保存
        EditorPrefs.SetFloat("TerrainRamp_Width", rampWidth);
        EditorPrefs.SetFloat("TerrainRamp_Falloff", falloff);
        EditorPrefs.SetFloat("TerrainRamp_Offset", offsetValue);
        EditorPrefs.SetInt("TerrainRamp_Axis", (int)offsetAxis);

        if (targetTerrain != null)
        {
            EditorPrefs.SetString("TerrainRamp_TerrainName", targetTerrain.name);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Terrain Ramp Generator", EditorStyles.boldLabel);

        // Terrainの変更監視 -> 保存
        EditorGUI.BeginChangeCheck();
        targetTerrain = (Terrain)EditorGUILayout.ObjectField("Target Terrain", targetTerrain, typeof(Terrain), true);
        if (EditorGUI.EndChangeCheck() && targetTerrain != null)
        {
            EditorPrefs.SetString("TerrainRamp_TerrainName", targetTerrain.name);
        }

        if (targetTerrain == null)
        {
            if (GUILayout.Button("Find Active Terrain"))
            {
                targetTerrain = Terrain.activeTerrain;
            }
        }

        EditorGUILayout.Space();

        // Start / End Point のアサイン
        startPoint = (Transform)EditorGUILayout.ObjectField("Start Point Transform", startPoint, typeof(Transform), true);
        endPoint = (Transform)EditorGUILayout.ObjectField("End Point Transform", endPoint, typeof(Transform), true);

        if (GUILayout.Button("Create Helper Points (if missing)"))
        {
            CreateHelperPoints();
        }

        EditorGUILayout.Space();

        // --- 座標編集機能 ---
        if (startPoint != null && endPoint != null)
        {
            GUILayout.Label("Coordinates Editor", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();

            Vector3 startPos = EditorGUILayout.Vector3Field("Start Position", startPoint.position);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(startPoint, "Move Start Point");
                startPoint.position = startPos;
            }

            EditorGUI.BeginChangeCheck();
            Vector3 endPos = EditorGUILayout.Vector3Field("End Position", endPoint.position);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(endPoint, "Move End Point");
                endPoint.position = endPos;
            }

            EditorGUILayout.Space();

            // EndをStartと同じにする
            if (GUILayout.Button("Startを基準にする (End ← Start)"))
            {
                Undo.RecordObject(endPoint, "Snap End to Start");
                endPoint.position = startPoint.position;
            }

            // StartをEndと同じにする
            if (GUILayout.Button("Endを基準にする (Start ← End)"))
            {
                Undo.RecordObject(startPoint, "Snap Start to End");
                startPoint.position = endPoint.position;
            }

            // Offset機能
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Add Offset to End Point:");
            offsetValue = EditorGUILayout.FloatField(offsetValue, GUILayout.Width(50));
            offsetAxis = (Axis)EditorGUILayout.EnumPopup(offsetAxis, GUILayout.Width(50));

            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                Undo.RecordObject(endPoint, "Add Offset");
                Vector3 current = endPoint.position;
                if (offsetAxis == Axis.X) current.x += offsetValue;
                else current.z += offsetValue; // Z
                endPoint.position = current;
            }
            EditorGUILayout.EndHorizontal();
        }
        // ------------------

        EditorGUILayout.Space();

        rampWidth = EditorGUILayout.Slider("Ramp Width", rampWidth, 1.0f, 50.0f);
        falloff = EditorGUILayout.Slider("Edge Falloff", falloff, 0.0f, 20.0f);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Ramp", GUILayout.Height(30)))
        {
            if (targetTerrain != null && startPoint != null && endPoint != null)
            {
                GenerateRamp();
            }
            else
            {
                Debug.LogError("Error: Please assign Terrain, Start Point, and End Point.");
            }
        }
    }

    private void CreateHelperPoints()
    {
        // 既存のポイントがなければ作成、あれば流用
        if (startPoint == null)
        {
            GameObject startObj = GameObject.Find("Ramp_Start");
            if (startObj == null)
            {
                startObj = new GameObject("Ramp_Start");
                // シーンビューの中心に置く
                if (SceneView.lastActiveSceneView != null)
                    startObj.transform.position = SceneView.lastActiveSceneView.pivot;
                Undo.RegisterCreatedObjectUndo(startObj, "Create Ramp Points");
            }
            startPoint = startObj.transform;
        }

        if (endPoint == null)
        {
            GameObject endObj = GameObject.Find("Ramp_End");
            if (endObj == null)
            {
                endObj = new GameObject("Ramp_End");
                // Startの少し先に置く
                endObj.transform.position = startPoint.position + Vector3.forward * 10f;
                Undo.RegisterCreatedObjectUndo(endObj, "Create Ramp Points");
            }
            endPoint = endObj.transform;
        }

        Selection.activeGameObject = startPoint.gameObject;
    }

    private void GenerateRamp()
    {
        TerrainData terrainData = targetTerrain.terrainData;
        Undo.RegisterCompleteObjectUndo(terrainData, "Generate Ramp");

        int heightMapRes = terrainData.heightmapResolution;
        Vector3 terrainSize = terrainData.size;

        Vector3 startLocal = targetTerrain.transform.InverseTransformPoint(startPoint.position);
        Vector3 endLocal = targetTerrain.transform.InverseTransformPoint(endPoint.position);

        Vector3 startNorm = new Vector3(startLocal.x / terrainSize.x, startLocal.y / terrainSize.y, startLocal.z / terrainSize.z);
        Vector3 endNorm = new Vector3(endLocal.x / terrainSize.x, endLocal.y / terrainSize.y, endLocal.z / terrainSize.z);

        float startMapX = startNorm.x * (heightMapRes - 1);
        float startMapZ = startNorm.z * (heightMapRes - 1);
        float endMapX = endNorm.x * (heightMapRes - 1);
        float endMapZ = endNorm.z * (heightMapRes - 1);

        Debug.Log($"Generating Ramp... H-Range: {startNorm.y:F4} -> {endNorm.y:F4}");

        float padding = (rampWidth + falloff) * ((float)heightMapRes / terrainSize.x) * 2.0f;
        int minX = Mathf.FloorToInt(Mathf.Min(startMapX, endMapX) - padding);
        int maxX = Mathf.CeilToInt(Mathf.Max(startMapX, endMapX) + padding);
        int minZ = Mathf.FloorToInt(Mathf.Min(startMapZ, endMapZ) - padding);
        int maxZ = Mathf.CeilToInt(Mathf.Max(startMapZ, endMapZ) + padding);

        minX = Mathf.Clamp(minX, 0, heightMapRes);
        maxX = Mathf.Clamp(maxX, 0, heightMapRes);
        minZ = Mathf.Clamp(minZ, 0, heightMapRes);
        maxZ = Mathf.Clamp(maxZ, 0, heightMapRes);

        int width = maxX - minX;
        int height = maxZ - minZ;

        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("Selected area is empty or outside terrain bounds.");
            return;
        }

        float[,] heights = terrainData.GetHeights(minX, minZ, width, height);

        Vector2 lineStart = new Vector2(startMapX, startMapZ);
        Vector2 lineEnd = new Vector2(endMapX, endMapZ);
        float lineLengthSq = (lineEnd - lineStart).sqrMagnitude;

        float mapToWorldScale = terrainSize.x / (heightMapRes - 1);

        int modifiedCount = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentMapX = minX + x;
                float currentMapZ = minZ + z;
                Vector2 currentPos = new Vector2(currentMapX, currentMapZ);

                Vector2 ap = currentPos - lineStart;
                Vector2 ab = lineEnd - lineStart;

                float t = 0;
                if (lineLengthSq > 0.0001f)
                {
                    t = Vector2.Dot(ap, ab) / lineLengthSq;
                    t = Mathf.Clamp01(t);
                }

                Vector2 closestPoint = lineStart + ab * t;
                float distMapUnits = Vector2.Distance(currentPos, closestPoint);
                float distWorldUnits = distMapUnits * mapToWorldScale;

                if (distWorldUnits <= rampWidth + falloff)
                {
                    float targetH = Mathf.Lerp(startNorm.y, endNorm.y, t);

                    float blend = 0f;
                    if (distWorldUnits <= rampWidth)
                    {
                        blend = 1.0f;
                    }
                    else
                    {
                        float f = (distWorldUnits - rampWidth) / falloff;
                        blend = 1.0f - Mathf.SmoothStep(0f, 1f, f);
                    }

                    float originalH = heights[z, x];
                    heights[z, x] = Mathf.Lerp(originalH, targetH, blend);

                    modifiedCount++;
                }
            }
        }

        terrainData.SetHeights(minX, minZ, heights);
        Debug.Log($"<color=green>Ramp Generated!</color> Modified {modifiedCount} points.");
    }
}
