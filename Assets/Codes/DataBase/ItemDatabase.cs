using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// ゲーム内の全ItemDataを管理するデータベース。
/// ContextMenuの「Auto Collect」で Assets 以下から全 ItemData を自動収集する。
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game Data/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private ItemData[] items;

    /// <summary>IDでItemDataを検索する</summary>
    public ItemData FindById(string id)
    {
        if (string.IsNullOrEmpty(id) || items == null) return null;
        foreach (var item in items)
            if (item != null && item.ID == id) return item;
        return null;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Assets以下の全ItemDataアセットを自動で収集してitemsに登録する。
    /// 新しいItemDataを追加したらInspectorで右クリック → "Auto Collect All ItemData" を実行。
    /// </summary>
    [ContextMenu("Auto Collect All ItemData")]
    private void AutoCollect()
    {
        var guids = AssetDatabase.FindAssets("t:ItemData");
        items = new ItemData[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            items[i] = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        }
        EditorUtility.SetDirty(this);
        Debug.Log($"[ItemDatabase] {items.Length} 件のItemDataを収集しました。");
    }
#endif
}
