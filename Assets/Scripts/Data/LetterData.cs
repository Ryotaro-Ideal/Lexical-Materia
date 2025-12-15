using UnityEngine;

public enum LetterType
{
    Hiragana,
    Katakana,
    Kanji,
    Alphabet
}
[CreateAssetMenu(fileName = "LetterData", menuName = "Scriptable Objects/LetterData")]
public class LetterData : ScriptableObject
{


    public LetterType type; // ひらがな、カタカナ、漢字、アルファベットなど

    public string letterName;

}
