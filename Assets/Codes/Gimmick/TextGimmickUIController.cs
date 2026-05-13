using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class TextGimmickUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup gimmickUI;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private LetterSlotManager[] letterSlots;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backspaceButton;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject slotPrefab;

    private InputHandler inputHandler;
    private PlayerInput playerInput;
    private LetterInvManager letterInvManager;

    private string playerMapName = "Player";
    private string uiMapName = "UI";

    private string correctWord;
    private UnityEvent onCorrectAction;

    private List<LetterSlotManager> inputSlots = new List<LetterSlotManager>();
    private List<TextMeshProUGUI> slotTexts = new List<TextMeshProUGUI>();

    private void Start()
    {
        inputHandler = InputHandler.Instance;
        if (inputHandler == null) inputHandler = FindFirstObjectByType<InputHandler>();
        playerInput = FindFirstObjectByType<PlayerInput>();
        letterSlots = GetComponentsInChildren<LetterSlotManager>();
        letterInvManager = LetterInvManager.Instance;

        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);
        if (backspaceButton != null) backspaceButton.onClick.AddListener(OnBackspaceClicked);

        if (letterSlots != null)
        {
            foreach (var slot in letterSlots)
            {
                slot.isGimmickMode = true;
                slot.OnGimmickClicked += OnLetterSlotClicked;
            }
        }

        CloseGimmickUI();
    }

    public void OpenGimmickUI(string answerWord, UnityEvent correctAction)
    {
        if (inputHandler == null || playerInput == null || gimmickUI == null) return;

        correctWord = answerWord;
        onCorrectAction = correctAction;
        inputSlots.Clear();
        if (resultText != null) resultText.text = "";

        inputHandler.IsGimmickUIOpened = true;
        inputHandler.OnGimmickUICloseRequested += HandleCloseRequested;
        playerInput.SwitchCurrentActionMap(uiMapName);
        if (CameraController.Instance != null) CameraController.Instance.SwitchLookControll(false);

        InitializeSlots();
        SyncWithRealInventory();

        if (confirmButton != null) confirmButton.interactable = true;
        if (backspaceButton != null) backspaceButton.interactable = true;

        gimmickUI.alpha = 1;
        gimmickUI.interactable = true;
        gimmickUI.blocksRaycasts = true;
    }

    private void InitializeSlots()
    {
        if (slotContainer == null || slotPrefab == null) return;

        foreach (Transform child in slotContainer) Destroy(child.gameObject);
        slotTexts.Clear();

        for (int i = 0; i < correctWord.Length; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            TextMeshProUGUI txt = slotObj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = "";
                slotTexts.Add(txt);
            }
        }
    }

    private void SyncWithRealInventory()
    {
        if (letterInvManager == null || letterSlots == null) return;

        var availableLetters = letterInvManager.GetAllAvailableLetters();

        foreach (var slot in letterSlots)
        {
            if (slot.letterData == null) continue;

            int count = 0;
            if (availableLetters.ContainsKey(slot.letterData))
            {
                count = availableLetters[slot.letterData];
            }

            slot.ResetCount();
            slot.AddCount(count);
            slot.GetComponent<Button>().interactable = (count > 0);
        }
    }

    private void OnLetterSlotClicked(LetterSlotManager clickedSlot)
    {
        if (inputSlots.Count >= correctWord.Length) return;
        if (clickedSlot.Count <= 0) return;

        clickedSlot.AddCount(-1);
        clickedSlot.GetComponent<Button>().interactable = (clickedSlot.Count > 0);
        inputSlots.Add(clickedSlot);

        slotTexts[inputSlots.Count - 1].text = clickedSlot.letterData.letterName;
    }

    private void OnBackspaceClicked()
    {
        if (inputSlots.Count == 0) return;

        LetterSlotManager lastSlot = inputSlots[inputSlots.Count - 1];
        inputSlots.RemoveAt(inputSlots.Count - 1);

        lastSlot.AddCount(1);
        lastSlot.GetComponent<Button>().interactable = true;

        slotTexts[inputSlots.Count].text = "";
    }

    private void OnConfirmClicked()
    {
        if (inputSlots.Count == 0) return;

        string currentWord = "";
        foreach (var s in inputSlots) currentWord += s.letterData.letterName;

        List<LetterData> lettersToConsume = new List<LetterData>();
        foreach (var s in inputSlots) lettersToConsume.Add(s.letterData);

        if (letterInvManager != null)
        {
            letterInvManager.ConsumeLetters(lettersToConsume);
        }

        if (currentWord == correctWord)
        {
            if (resultText != null) resultText.text = "正解！";
            onCorrectAction?.Invoke();
            Invoke("CloseGimmickUI", 1.5f);
        }
        else
        {
            if (resultText != null) resultText.text = "間違っている…";
            Invoke("ResetForRetry", 1.5f);
        }

        foreach (var slot in letterSlots) slot.GetComponent<Button>().interactable = false;
        if (confirmButton != null) confirmButton.interactable = false;
        if (backspaceButton != null) backspaceButton.interactable = false;
    }

    private void ResetForRetry()
    {
        if (resultText != null) resultText.text = "";
        inputSlots.Clear();
        foreach (var txt in slotTexts) txt.text = "";

        if (confirmButton != null) confirmButton.interactable = true;
        if (backspaceButton != null) backspaceButton.interactable = true;

        SyncWithRealInventory();
    }

    public void CloseGimmickUI()
    {
        CancelInvoke();
        if (gimmickUI == null) return;

        gimmickUI.alpha = 0;
        gimmickUI.interactable = false;
        gimmickUI.blocksRaycasts = false;

        if (inputHandler != null)
        {
            inputHandler.IsGimmickUIOpened = false;
            inputHandler.OnGimmickUICloseRequested -= HandleCloseRequested;
        }

        if (playerInput != null) playerInput.SwitchCurrentActionMap(playerMapName);
        if (CameraController.Instance != null) CameraController.Instance.SwitchLookControll(true);
    }

    private void HandleCloseRequested()
    {
        CloseGimmickUI();
    }
}
