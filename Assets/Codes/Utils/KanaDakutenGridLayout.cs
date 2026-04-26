using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;

/// <summary>
/// 濁音・半濁音・小文字のグリッドレイアウト。
/// presetでひらがな/カタカナを切り替えると columnsDefinition が自動設定される。
/// Customを選ぶと手動で列定義できる。
/// </summary>
public class KanaDakutenGridLayout : MonoBehaviour
{
    public enum KanaPreset
    {
        HiraganaDakuten,  // ひらがな：濁音・半濁音・小文字
        KatakanaDakuten,  // カタカナ：濁音・半濁音・小文字
        Custom            // 手動で列定義
    }

    [Header("プリセット")]
    public KanaPreset preset = KanaPreset.HiraganaDakuten;

    [Header("セルレイアウト")]
    public Vector2 cellSize = new Vector2(80, 80);
    public Vector2 spacing = new Vector2(20, 20);
    public Vector2 padding = new Vector2(0, 0);

    [Header("Custom のときだけ使用する列定義")]
    public string[][] customColumnsDefinition;

    [Header("Character 書き換え設定")]
    public bool setCharacterText = true;
    public string characterChildName = "Character";

    // ------- プリセットデータ -------

    private static readonly string[][] HiraganaColumns = new string[][]
    {
        new string[] { "が","ぎ","ぐ","げ","ご" }, // が行
        new string[] { "ざ","じ","ず","ぜ","ぞ" }, // ざ行
        new string[] { "だ","ぢ","づ","で","ど" }, // だ行
        new string[] { "ば","び","ぶ","べ","ぼ" }, // ば行
        new string[] { "ぱ","ぴ","ぷ","ぺ","ぽ" }, // ぱ行（半濁音）
        new string[] { "ぁ","ぃ","ぅ","ぇ","ぉ" }, // 小母音
        new string[] { "っ","ゃ","ゅ","ょ","ゎ" }, // 小子音・拗音
    };

    private static readonly string[][] KatakanaColumns = new string[][]
    {
        new string[] { "ガ","ギ","グ","ゲ","ゴ" }, // ガ行
        new string[] { "ザ","ジ","ズ","ゼ","ゾ" }, // ザ行
        new string[] { "ダ","ヂ","ヅ","デ","ド" }, // ダ行
        new string[] { "バ","ビ","ブ","ベ","ボ" }, // バ行
        new string[] { "パ","ピ","プ","ペ","ポ" }, // パ行（半濁音）
        new string[] { "ァ","ィ","ゥ","ェ","ォ" }, // 小母音
        new string[] { "ッ","ャ","ュ","ョ","ヮ" }, // 小子音・拗音
    };

    // -----------------------------------


    [ContextMenu("Rebuild Dakuten Grid")]
    public void Rebuild()
    {
        string[][] columns = GetColumns();
        if (columns == null || columns.Length == 0) return;

        var parentRT = transform as RectTransform;
        if (parentRT == null) return;

        int totalNeeded = 0;
        foreach (var col in columns) if (col != null) totalNeeded += col.Length;

        int childCount = transform.childCount;
        if (childCount == 0)
        {
            Debug.LogWarning("KanaDakutenGridLayout: 子スロットがありません。");
            return;
        }

        float cellStepX = cellSize.x + spacing.x;
        float cellStepY = cellSize.y + spacing.y;
        int colCount = columns.Length;
        int childIndex = 0;

        for (int colDefIndex = 0; colDefIndex < colCount; colDefIndex++)
        {
            var col = columns[colDefIndex];
            if (col == null) continue;

            int colFromLeft = (colCount - 1) - colDefIndex;

            for (int rowInCol = 0; rowInCol < col.Length; rowInCol++)
            {
                if (childIndex >= childCount) break;

                var child = transform.GetChild(childIndex) as RectTransform;
                if (child == null) { childIndex++; continue; }

                float x = padding.x + colFromLeft * cellStepX;
                float y = -(padding.y + rowInCol * cellStepY);

                child.anchoredPosition = new Vector2(x, y);
                child.sizeDelta = cellSize;

                if (setCharacterText && rowInCol < col.Length)
                {
                    Transform character = child.Find(characterChildName);
                    if (character != null)
                    {
                        var tmp = character.GetComponent<TMP_Text>();
                        if (tmp != null)
                        {
                            tmp.text = col[rowInCol];
#if UNITY_EDITOR
                            if (!Application.isPlaying) EditorUtility.SetDirty(tmp);
#endif
                        }
                    }
                }

                childIndex++;
            }

            if (childIndex >= childCount) break;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SceneView.RepaintAll();
            EditorApplication.QueuePlayerLoopUpdate();
        }
#endif
    }

    private string[][] GetColumns()
    {
        return preset switch
        {
            KanaPreset.HiraganaDakuten => HiraganaColumns,
            KanaPreset.KatakanaDakuten => KatakanaColumns,
            KanaPreset.Custom => customColumnsDefinition,
            _ => HiraganaColumns,
        };
    }
}
