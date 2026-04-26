using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private static readonly string SaveFileName = "save.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeEvents();
    }

    /// <summary>シーンがロードされるたびに呼ばれる</summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1フレーム待ってからManagerの初期化が完了した後に実行
        StartCoroutine(DelayedSetup());
    }

    private IEnumerator DelayedSetup()
    {
        yield return null; // Managerの Awake/Start が完了するのを待つ

        UnsubscribeEvents();
        SubscribeEvents();
        LoadAll();
    }

    private void SubscribeEvents()
    {
        if (InventorySlotManager.Instance != null)
            InventorySlotManager.Instance.OnInventoryChanged += SaveAll;

        if (LetterInvManager.Instance != null)
            LetterInvManager.Instance.OnLetterChanged += SaveAll;
    }

    private void UnsubscribeEvents()
    {
        if (InventorySlotManager.Instance != null)
            InventorySlotManager.Instance.OnInventoryChanged -= SaveAll;

        if (LetterInvManager.Instance != null)
            LetterInvManager.Instance.OnLetterChanged -= SaveAll;
    }

    /// <summary>全データを保存する（イベント経由で自動呼び出し）</summary>
    public void SaveAll()
    {
        SaveData data = new SaveData();

        if (LetterInvManager.Instance != null)
            LetterInvManager.Instance.WriteSaveData(data);

        if (InventorySlotManager.Instance != null)
            InventorySlotManager.Instance.WriteSaveData(data);

        WriteToFile(data);
        Debug.Log($"[SaveManager] 自動保存: {SavePath}");
    }

    /// <summary>死亡時：文字だけ保持してアイテムをクリアして保存する</summary>
    public void ClearItemSaveAndKeepLetters()
    {
        SaveData data = new SaveData();

        // 文字だけ書く（アイテムリストは空 = クリア扱い）
        if (LetterInvManager.Instance != null)
            LetterInvManager.Instance.WriteSaveData(data);

        WriteToFile(data);
        Debug.Log("[SaveManager] 死亡処理：アイテムをクリアして文字を保持");
    }

    // ===== 読み込み =====

    /// <summary>セーブデータを読み込んで各Managerに反映する</summary>
    public void LoadAll()
    {
        SaveData data = ReadFromFile();
        if (data == null) return;

        if (LetterInvManager.Instance != null)
            LetterInvManager.Instance.ReadSaveData(data);

        if (InventorySlotManager.Instance != null)
            InventorySlotManager.Instance.ReadSaveData(data);

        Debug.Log("[SaveManager] ロード完了");
    }

    // ===== ファイル操作 =====

    private void WriteToFile(SaveData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
    }

    private SaveData ReadFromFile()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveManager] セーブファイルなし。新規扱いで進めます。");
            return null;
        }
        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    /// <summary>セーブファイルを削除する（デバッグ・リセット用）</summary>
    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[SaveManager] セーブファイルを削除しました");
        }
    }
}

