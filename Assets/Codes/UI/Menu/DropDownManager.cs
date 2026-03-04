using UnityEngine;
using TMPro;
public enum CharacterInventoryType
{
    Hiragana,
    Katakana,
    Alphabet
}

public class DropDownManager : MonoBehaviour
{
    [Header("Dropdown")]
    [SerializeField] private TMP_Dropdown dropdown;

    [Header("Inventory Roots")]
    [SerializeField] private GameObject hiraganaRoot;
    [SerializeField] private GameObject katakanaRoot;
    [SerializeField] private GameObject alphabetRoot;

    private void Awake()
    {
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();

        dropdown.onValueChanged.AddListener(OnDropdownChanged);

        // 初期表示
        ApplySelection(dropdown.value);
    }

    private void OnDestroy()
    {
        dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }

    private void OnDropdownChanged(int index)
    {
        ApplySelection(index);
    }

    private void ApplySelection(int index)
    {
        SetAllInactive();

        switch ((CharacterInventoryType)index)
        {
            case CharacterInventoryType.Hiragana:
                hiraganaRoot.SetActive(true);
                break;

            case CharacterInventoryType.Katakana:
                katakanaRoot.SetActive(true);
                break;

            case CharacterInventoryType.Alphabet:
                alphabetRoot.SetActive(true);
                break;
        }
    }

    private void SetAllInactive()
    {
        if (hiraganaRoot != null) hiraganaRoot.SetActive(false);
        if (katakanaRoot != null) katakanaRoot.SetActive(false);
        if (alphabetRoot != null) alphabetRoot.SetActive(false);
    }
}
