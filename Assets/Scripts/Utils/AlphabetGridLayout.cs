using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;

[ExecuteAlways]
public class AlphabetGridLayout : MonoBehaviour
{
    [Header("セルレイアウト")]
    public Vector2 cellSize = new Vector2(80, 80);
    public Vector2 spacing = new Vector2(30, 30);
    public Vector2 padding = new Vector2(0, 0); // left, top

    [Header("QWERTY 行定義（上→下）")]
    public string[][] rowDefinitions = new string[][]
    {
        new string[] {"1","2","3","3","4","5","6","7","8","9","0"},
        new string[] { "Q","W","E","R","T","Y","U","I","O","P" },
        new string[] { "A","S","D","F","G","H","J","K","L" },
        new string[] { "Z","X","C","V","B","N","M" }
    };

    [Header("行ごとの X オフセット（キー段差）")]
    public float[] rowOffsetX = new float[]
    {
        0f,     // 1234
        0.5f,   // QWER
        1.0f,   // ASDF
        1.5f,   //ZXCV
    };

    [Header("Character 書き換え設定")]
    public bool setCharacterText = true;
    public string characterChildName = "Character";

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

    [ContextMenu("Rebuild Alphabet Grid (QWERTY)")]
    public void Rebuild()
    {
        var parentRT = transform as RectTransform;
        if (parentRT == null)
        {
            Debug.LogWarning("AlphabetGridLayout: RectTransform が必要です。");
            return;
        }

        int totalNeeded = 0;
        foreach (var row in rowDefinitions)
            if (row != null) totalNeeded += row.Length;

        int childCount = transform.childCount;
        if (childCount == 0) return;

        float stepX = cellSize.x + spacing.x;
        float stepY = cellSize.y + spacing.y;

        int childIndex = 0;

        for (int rowIndex = 0; rowIndex < rowDefinitions.Length; rowIndex++)
        {
            var row = rowDefinitions[rowIndex];
            if (row == null) continue;

            float offsetX = 0f;
            if (rowIndex < rowOffsetX.Length)
                offsetX = rowOffsetX[rowIndex] * stepX;

            for (int col = 0; col < row.Length; col++)
            {
                if (childIndex >= childCount) break;

                var child = transform.GetChild(childIndex) as RectTransform;
                if (child == null)
                {
                    childIndex++;
                    continue;
                }

                float x = padding.x + offsetX + col * stepX;
                float y = -(padding.y + rowIndex * stepY);

                child.anchoredPosition = new Vector2(x, y);
                child.sizeDelta = cellSize;

                if (setCharacterText)
                {
                    Transform character = child.Find(characterChildName);
                    if (character != null)
                    {
                        var tmp = character.GetComponent<TMP_Text>();
                        if (tmp != null)
                        {
                            tmp.text = row[col];
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
}
