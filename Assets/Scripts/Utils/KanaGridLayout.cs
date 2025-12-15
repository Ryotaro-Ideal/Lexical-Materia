using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;


[ExecuteAlways]
public class KanaGridLayouut : MonoBehaviour
{
    [Header("セルレイアウト")]
    public Vector2 cellSize = new Vector2(64, 64);
    public Vector2 spacing = new Vector2(8, 8);
    public Vector2 padding = new Vector2(8, 8); // left, top

    [Header("列定義（右→左の順で列を並べる）")]

    public string[][] columnsDefinition = new string[][]
    {
        new string[] { "あ","い","う","え","お" }, // 右端: あ行
        new string[] { "か","き","く","け","こ" }, // その左: か行
        new string[] { "さ","し","す","せ","そ" },
        new string[] { "た","ち","つ","て","と" },
        new string[] { "な","に","ぬ","ね","の" },
        new string[] { "は","ひ","ふ","へ","ほ" },
        new string[] { "ま","み","む","め","も" },
        new string[] { "や","ゆ","よ" },          // 短い列 (3)
        new string[] { "ら","り","る","れ","ろ" },
        new string[] { "わ","を","ん" }           // 短い列 (3) - 左端
    };

    [Header("Character 書き換え設定")]
    public bool setCharacterText = true;
    public string characterChildName = "Character"; // 見つける子オブジェクト名（厳密一致）

    // エディタでの即時反映
    private void OnValidate()
    {
        if (cellSize.x <= 0) cellSize.x = 1;
        if (cellSize.y <= 0) cellSize.y = 1;
        if (spacing.x < 0) spacing.x = 0;
        if (spacing.y < 0) spacing.y = 0;
        if (padding.x < 0) padding.x = 0;
        if (padding.y < 0) padding.y = 0;

        Rebuild();
    }

    [ContextMenu("Rebuild Kana Grid (ColumnMajor)")]
    public void Rebuild()
    {
        var parentRT = transform as RectTransform;
        if (parentRT == null)
        {
            Debug.LogWarning("KanaGridPlacer_ColumnMajor: RectTransform を持つオブジェクトにアタッチしてください。");
            return;
        }

        // トータル必要セル数を計算
        int totalNeeded = 0;
        if (columnsDefinition != null)
        {
            foreach (var col in columnsDefinition) if (col != null) totalNeeded += col.Length;
        }

        // 子の数
        int childCount = transform.childCount;
        if (childCount == 0)
        {
            Debug.LogWarning("KanaGridPlacer_ColumnMajor: 子スロットがありません。");
            return;
        }

        if (childCount < totalNeeded)
        {
            Debug.LogWarning($"KanaGridPlacer_ColumnMajor: 子スロット数({childCount}) が必要セル数({totalNeeded}) より少ないです。足りない分は生成されません。");
        }
        else if (childCount > totalNeeded)
        {
            // 多すぎても良い（余分は空セルとして残す）
        }

        // 列数
        int colCount = columnsDefinition != null ? columnsDefinition.Length : 0;
        if (colCount == 0) return;

        float cellStepX = cellSize.x + spacing.x;
        float cellStepY = cellSize.y + spacing.y;

        int childIndex = 0;

        for (int colDefIndex = 0; colDefIndex < colCount; colDefIndex++)
        {
            var col = columnsDefinition[colDefIndex];
            if (col == null) continue;
            int colsElements = col.Length;

            // 左からの列番号を計算（0 が左端）
            int colFromLeft = (colCount - 1) - colDefIndex;

            for (int rowInCol = 0; rowInCol < colsElements; rowInCol++)
            {
                if (childIndex >= childCount) break; // 子が尽きたら終わり
                // rowInCol: 0 = top, increasing goes downward

                var child = transform.GetChild(childIndex) as RectTransform;
                if (child == null)
                {
                    childIndex++;
                    continue;
                }

                float x = padding.x + colFromLeft * cellStepX;
                float y = -(padding.y + rowInCol * cellStepY);

                child.anchoredPosition = new Vector2(x, y);
                child.sizeDelta = cellSize;

                // Character 子を書き換え
                if (setCharacterText)
                {
                    if (rowInCol < col.Length)
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
                        else
                        {
                            // Character 子が無い場合は無視（安全）
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
}
