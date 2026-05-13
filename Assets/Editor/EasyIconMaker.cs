using UnityEngine;
using UnityEditor;
using System.IO;

public class EasyIconMaker : EditorWindow
{
    float distanceMultiplier = 2.0f;
    int resolution = 256;

    [MenuItem("Window/EasyIconMaker")]
    public static void OpenWindow()
    {
        GetWindow<EasyIconMaker>("EasyIconMaker");
    }

    void OnGUI()
    {
        GUILayout.Label("アイコン生成設定", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        distanceMultiplier = EditorGUILayout.Slider(
            new GUIContent(
                "カメラ距離倍率",
                "小さく見える → 値を下げる  /  大きく見える → 値を上げる"),
            distanceMultiplier, 0.5f, 10f);

        resolution = EditorGUILayout.IntPopup(
            "解像度",
            resolution,
            new[] { "64px", "128px", "256px", "512px", "1024px" },
            new[] { 64, 128, 256, 512, 1024 });

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Projectウィンドウでプレハブを選択してからボタンを押してください。",
            MessageType.Info);

        GUI.enabled = Selection.activeGameObject != null;
        if (GUILayout.Button("アイコン生成（8方向）", GUILayout.Height(36)))
            Generate();
        GUI.enabled = true;
    }

    void Generate()
    {
        GameObject prefab = Selection.activeGameObject;
        if (prefab == null)
        {
            Debug.LogWarning("Prefabを選択してください");
            return;
        }

        string prefabAssetPath = AssetDatabase.GetAssetPath(prefab);
        if (string.IsNullOrEmpty(prefabAssetPath))
        {
            Debug.LogWarning("ProjectウィンドウからPrefabを選択してください（シーン上のオブジェクトは不可）");
            return;
        }
        string saveDir = Path.GetDirectoryName(prefabAssetPath);

        int iconLayer = LayerMask.NameToLayer("Icon");
        if (iconLayer == -1)
        {
            Debug.LogError("Layer 'Icon' がありません。Project Settings > Tags and Layers で作成する必要があります。");
            return;
        }

        Vector3 workPos = new Vector3(0, -900, 0);
        GameObject instance = Object.Instantiate(prefab, workPos, prefab.transform.rotation);
        SetLayerRecursively(instance, iconLayer);

        Bounds bounds = new Bounds(instance.transform.position, Vector3.zero);
        bool hasBounds = false;
        foreach (Renderer r in instance.GetComponentsInChildren<Renderer>())
        {
            if (r is SkinnedMeshRenderer smr) smr.updateWhenOffscreen = true;
            if (!hasBounds) { bounds = r.bounds; hasBounds = true; }
            else bounds.Encapsulate(r.bounds);
        }

        if (!hasBounds)
        {
            Debug.LogWarning("Rendererが見つかりませんでした。");
            Object.DestroyImmediate(instance);
            return;
        }

        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float distance = Mathf.Max(maxSize * distanceMultiplier, 1f);

        GameObject camObj = new GameObject("TempIconCamera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.cullingMask = 1 << iconLayer;
        cam.transform.position = bounds.center + new Vector3(0, distance * 0.5f, -distance);
        cam.transform.LookAt(bounds.center);

        RenderTexture rt = RenderTexture.GetTemporary(resolution, resolution, 24);
        cam.targetTexture = rt;

        Quaternion baseRotation = instance.transform.rotation;
        string[] savedPaths = new string[8];

        for (int i = 0; i < 8; i++)
        {
            float yAngle = i * 45f;
            instance.transform.rotation = baseRotation * Quaternion.Euler(0, yAngle, 0);

            cam.Render();

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            string fileName = $"{prefab.name}_icon_{(int)yAngle}.png";
            string fullPath = Path.Combine(saveDir, fileName).Replace("\\", "/");
            File.WriteAllBytes(fullPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            savedPaths[i] = fullPath;
        }

        cam.targetTexture = null;
        RenderTexture.ReleaseTemporary(rt);
        Object.DestroyImmediate(camObj);
        Object.DestroyImmediate(instance);

        AssetDatabase.Refresh();

        foreach (string assetPath in savedPaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) continue;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }

        Debug.Log($"<b>[EasyIconMaker]</b> {savedPaths.Length}枚のSpriteを生成しました → {saveDir}");
    }

    static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }
}
