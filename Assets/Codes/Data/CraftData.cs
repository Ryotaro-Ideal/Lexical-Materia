using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftData", menuName = "Scriptable Objects/CraftData")]
public class CraftData : ScriptableObject
{
    public ItemData itemData;
    public List<DestroyMaterial> requiredLetters = new List<DestroyMaterial>();
}



