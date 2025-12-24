using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftData", menuName = "Scriptable Objects/CraftData")]
public class CraftData : ScriptableObject
{
    public ItemData itemData;
    public List<RequiredLetter> requiredLetters = new List<RequiredLetter>();
}
[System.Serializable]
public class RequiredLetter
{
    public LetterData letterData;
    public int count;
}

