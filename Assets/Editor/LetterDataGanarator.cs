using UnityEngine;
using UnityEditor; // エディタ拡張に必要な名前空間
using System.IO;

public class LetterDataGenerator : EditorWindow
{
    // 自動生成する文字のリスト
    private string hiraganaList = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをんがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽゃゅょっ";
    private string katakanaList = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポャュョッ";
    private string generationPath = "Assets/Assets/ScriptableObjects/Letters/";

    // ウィンドウメニューに追加
    [MenuItem("Tools/Inventory/Generate Character Data Assets")]
    public static void ShowWindow()
    {
        GetWindow<LetterDataGenerator>("Character Data Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Character Data Asset Generator", EditorStyles.boldLabel);

        generationPath = EditorGUILayout.TextField("Generation Path", generationPath);

        GUILayout.Space(10);

        // --- ひらがな生成ボタン ---
        if (GUILayout.Button("Generate Hiragana Assets"))
        {
            GenerateAssets(hiraganaList, LetterType.Hiragana);
        }

        // --- カタカナ生成ボタン ---
        if (GUILayout.Button("Generate Katakana Assets"))
        {
            GenerateAssets(katakanaList, LetterType.Katakana);
        }
    }

    private void GenerateAssets(string characterList, LetterType type)
    {
        // パスが存在するか確認し、なければ作成
        if (!Directory.Exists(generationPath))
        {
            Directory.CreateDirectory(generationPath);
            AssetDatabase.Refresh(); // Unityエディタに新しいフォルダを認識させる
        }

        // ログの開始
        int createdCount = 0;

        foreach (char c in characterList)
        {
            string charName = c.ToString();
            string assetPath = generationPath + type.ToString() + "_" + charName + ".asset";

            // 既にアセットが存在するか確認（重複防止）
            if (AssetDatabase.LoadAssetAtPath<LetterData>(assetPath) != null)
            {
                // Debug.LogWarning($"Asset already exists: {assetPath}");
                continue;
            }

            // 新しいScriptableObjectインスタンスを作成
            LetterData newCharData = ScriptableObject.CreateInstance<LetterData>();

            // データを設定
            newCharData.type = type;
            newCharData.letterName = charName;

            // アセットとして保存
            AssetDatabase.CreateAsset(newCharData, assetPath);
            createdCount++;
        }

        // 全てのアセットの変更を保存し、エディタを更新
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"--- {type} Assets Generated Successfully! ---");
        Debug.Log($"Total {createdCount} new assets created in: {generationPath}");
    }
}