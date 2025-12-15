using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class LetterInventory : MonoBehaviour
{
    private Dictionary<string, int> letterInventory = new Dictionary<string, int>();

    public IReadOnlyDictionary<string, int> GetAllMaterials() => letterInventory; //所持数デバッグ

    public void AddLetter(string letterName, int count = 1)
    {
        if (string.IsNullOrEmpty(letterName) || count <= 0) return;

        if (letterInventory.ContainsKey(letterName))
        {
            letterInventory[letterName] += count;
        }
        else
        {
            letterInventory.Add(letterName, count);
        }
        
        // 通常はここでUI更新イベントを発火しますが、プロトタイプではDebug.Logで確認
        Debug.Log($"[Letter] {letterName}を {count} 個取得。合計: {letterInventory[letterName]}");
    }

    public int CheckLetterCount(string letterName)
    {
        return letterInventory.ContainsKey(letterName) ? letterInventory[letterName] : 0;
    }

    public bool HasRequiredLetters(List<string> requiredLetters)
    {
        if (requiredLetters == null || requiredLetters.Count == 0) return true;

        // 全ての必要な素材が所持リストにあるか、かつ数が足りているかチェック
        foreach (string letter in requiredLetters)
        {
            if (CheckLetterCount(letter) <= 0)
            {
                // デバッグ用: 何が足りないか表示
                Debug.LogWarning($"クラフトに必要な素材が不足しています: {letter}");
                return false;
            }
        }
        return true;
    }

    public bool ConsumeLetters(List<string> lettersToConsume)
    {
        if (lettersToConsume == null || lettersToConsume.Count == 0) return true;

        // 必須チェック: 消費可能か確認してから消費処理に入る
        if (!HasRequiredLetters(lettersToConsume))
        {
            return false;
        }

        foreach (string letter in lettersToConsume)
        {
            letterInventory[letter]--;
            // 所持数がゼロになった文字はリストから削除
            if (letterInventory[letter] <= 0)
            {
                letterInventory.Remove(letter);
            }
        }

        Debug.Log("[Letters] クラフトのために素材を消費しました。");
        return true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}