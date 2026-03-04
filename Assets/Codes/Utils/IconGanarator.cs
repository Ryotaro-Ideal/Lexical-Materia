using UnityEngine;
using UnityEditor;
using System.IO;

public class IconGenerator : MonoBehaviour
{
    [Header("設定")]
    public Camera renderCamera;          // 撮影に使うカメラ
    public int iconResolution = 256;     // 画像サイズ
    public string savePath = "Assets/Icons/"; // 保存フォルダ

    [Header("ファイル名設定")]
    public string fileName = "NewIcon"; // 保存するファイル名（拡張子なし）

    public void GenerateIcon()
    {
        if (renderCamera == null)
        {
            Debug.LogError("Render Camera が設定されていません！");
            return;
        }

        // RenderTextureの準備
        RenderTexture rt = new RenderTexture(iconResolution, iconResolution, 24);

        // カメラの設定を一瞬だけ撮影用に上書き（ターゲットテクスチャ）
        RenderTexture prevTarget = renderCamera.targetTexture;
        renderCamera.targetTexture = rt;

        // 撮影！
        renderCamera.Render();

        // 撮影データをTexture2Dに吸い出す
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        // 後片付け（カメラ設定を元に戻す）
        RenderTexture.active = null;
        renderCamera.targetTexture = prevTarget;
        rt.Release();

        // PNGとして保存
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
        byte[] bytes = tex.EncodeToPNG();
        string fullPath = savePath + fileName + ".png";

        File.WriteAllBytes(fullPath, bytes);
        AssetDatabase.Refresh();

        Debug.Log($"<b>[IconGenerator]</b> Saved: {fullPath}");
    }
}
