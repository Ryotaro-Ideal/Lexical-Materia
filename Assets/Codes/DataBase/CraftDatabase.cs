using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftDatabase", menuName = "Game Data/Craft Database")]
public class CraftDatabase : ScriptableObject
{
    public List<CraftData> allRecipes = new List<CraftData>();

    // ID順にソートしたリストを返す（例）
    public List<CraftData> GetSortedRecipes()
    {
        // 1:Tool, 2:Con, 3:Mat のルール等に基づいてソート
        return allRecipes.OrderBy(r => r.itemData.ID).ToList();
    }
}
