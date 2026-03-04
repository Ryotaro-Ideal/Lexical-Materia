using UnityEngine;
using UnityEditor;
using System.IO;

public class EasyIconMaker
{
    [MenuItem("Assets/Make Icon from Prefab (Transparent)")]
    public static void MakeTransparentIcon()
    {
        GameObject prefab = Selection.activeGameObject;
        if (prefab == null)
        {
            Debug.LogWarning("Prefabを選択してください");
            return;
        }

        // 1. シーンの下の方（邪魔にならない場所）で作業する
        Vector3 workPos = new Vector3(0, -900, 0);

        // 2. モデルを生成
        GameObject instance = Object.Instantiate(prefab, workPos, Quaternion.identity);

        // レイヤーをIconに変更（カメラに映すため）
        int iconLayer = LayerMask.NameToLayer("Icon");
        if (iconLayer == -1)
        {
            Debug.LogError("Layer 'Icon' がありません。Project Settings > Tags and Layers で作成する必要があります。");
            Object.DestroyImmediate(instance);
            return;
        }
        SetLayerRecursively(instance, iconLayer);

        // 3. バウンディングボックス（大きさ）を計算してカメラ位置を決める
        Bounds bounds = new Bounds(instance.transform.position, Vector3.zero);
        bool hasBounds = false;
        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (!hasBounds)
            {
                bounds = r.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(r.bounds);
            }
            // SkinnedMeshRendererが一瞬で消えるのを防ぐおまじない
            if (r is SkinnedMeshRenderer smr) smr.updateWhenOffscreen = true;
        }

        if (!hasBounds)
        {
            Debug.LogWarning("Rendererが見つかりませんでした。空のオブジェクトですか？");
            Object.DestroyImmediate(instance);
            return;
        }

        // カメラ作成
        GameObject camObj = new GameObject("TempIconCamera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0); // 透明
        cam.cullingMask = 1 << iconLayer;

        // 【重要】オブジェクトの正面斜め上から見るように配置
        // 中心点(bound.center)から、少し手前に引く
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float distance = maxSize * 2.0f; // 大きさに合わせて距離調整
        if (distance < 1f) distance = 1f;

        // カメラを配置：斜め上からのアングル
        cam.transform.position = bounds.center + new Vector3(0, distance * 0.5f, -distance);
        cam.transform.LookAt(bounds.center);

        // 少し回転させて見栄え良くする
        instance.transform.rotation = Quaternion.Euler(0, -30f, 0);
        // 回転させたのでBoundsが変わるかもしれないが、大体は収まるはず

        // 4. 撮影
        int resolution = 256;
        RenderTexture rt = RenderTexture.GetTemporary(resolution, resolution, 24);
        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        tex.Apply();

        // 5. 保存
        byte[] bytes = tex.EncodeToPNG();
        string path = $"Assets/Icons/{prefab.name}_icon.png";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllBytes(path, bytes);

        // 6. 後片付け
        RenderTexture.active = null;
        cam.targetTexture = null;
        RenderTexture.ReleaseTemporary(rt);
        Object.DestroyImmediate(camObj);
        Object.DestroyImmediate(instance);

        AssetDatabase.Refresh();
        Debug.Log($"<b>[EasyIconMaker]</b> Created: {path}");
    }

    private static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
