using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum ItemType
{
    Consumable,     // 消費アイテム（食べ物やポーションなど）
    Tool,      // 装備品（斧、防具など）
    Material,       // 分解前の収集アイテム（木の枝など）
}

[CreateAssetMenu(fileName = "NewItemData", menuName = "Game Data/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("基本情報")]

    [Header("識別子: (1:Tool, 2:Consumable, 3:Material) + 3桁の数字")]
    public string ID;
    public string itemName;

    public GameObject visualPrefab;
    public GameObject prefab;
    public Sprite icon;
    public ItemType itemType;

    [TextArea]
    public string description;

    [Header("クラフト・分解情報")]

    public int maxStack = 99;


    public List<LetterData> destroyMaterials = new List<LetterData>();



    [Header("装備性能 (Equipment Typeの場合のみ使用)")]
    public int baseAttackPower = 0;
    public int baseDefendPower = 0;

}