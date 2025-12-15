using UnityEngine;
using UnityEditor;
using System.IO;

public class IconGenerator : MonoBehaviour
{
    [Header("3Dモデルをここにセット")]
    public GameObject targetPrefab;      // アイコンを作りたいPrefab
    public Camera renderCamera;          // 専用カメラ（シーン上に置く）
    public int iconResolution = 256;     // PNGサイズ
    public string savePath = "Assets/Icons/"; // 保存先フォルダ

    public void GenerateIcon()
    {
        if (targetPrefab == null || renderCamera == null)
        {
            Debug.LogWarning("Prefab または Camera が設定されていません");
            return;
        }

        // RenderTexture作成
        RenderTexture rt = new RenderTexture(iconResolution, iconResolution, 24);
        renderCamera.targetTexture = rt;

        // 一時的にPrefabをシーンに配置
        GameObject temp = Instantiate(targetPrefab, Vector3.zero, Quaternion.identity);
        temp.transform.rotation = Quaternion.Euler(30f, 45f, 0f); // 見やすい角度に調整

        // Render
        renderCamera.Render();

        // Texture2D にコピー
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        // Sprite に変換
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        // PNG 保存
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
        byte[] bytes = tex.EncodeToPNG();
        string fileName = savePath + targetPrefab.name + ".png";
        File.WriteAllBytes(fileName, bytes);
        AssetDatabase.Refresh();

        Debug.Log($"Icon generated: {fileName}");

        // 後処理
        DestroyImmediate(temp);
        renderCamera.targetTexture = null;
        rt.Release();
    }
}
