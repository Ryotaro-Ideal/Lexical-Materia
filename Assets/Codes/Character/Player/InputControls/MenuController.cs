using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    public bool IsMenuOpen { get; private set; } = false;
    private PlayerInput playerInput;
    private InputHandler inputHandler;
    private CanvasGroup menuUI;


    public CanvasGroup SlotsExplainCanvas;
    public CanvasGroup ItemExplainCanvas;

    CameraController cameraController => CameraController.Instance;


    [Header("マップ名（ActionMap 名）")]
    private string playerMapName = "Player";   // 既存プロジェクトのマップ名に合わせる
    private string uiMapName = "UI";

    private void Awake()
    {
        menuUI = GetComponent<CanvasGroup>();
        playerInput = FindFirstObjectByType<PlayerInput>();
        inputHandler = FindFirstObjectByType<InputHandler>();
        CloseMenu();
        SwitchExplainCanvas(ItemExplainCanvas);
    }
    private void Update()
    {
        if (inputHandler == null || playerInput == null) return;

        // メニュー開閉入力を監視
        if (inputHandler.IsMenuOpened && !IsMenuOpen)
        {
            IsMenuOpen = !IsMenuOpen;
            SoundManager.Instance.PlaySE(SoundName.MenuOpen);
            OpenMenu();
            SwitchExplainCanvas(SlotsExplainCanvas);
            cameraController.SwitchLookControll(false);
        }
        else if (!inputHandler.IsMenuOpened && IsMenuOpen)
        {
            IsMenuOpen = !IsMenuOpen;
            SoundManager.Instance.PlaySE(SoundName.MenuOpen);
            CloseMenu();
            SwitchExplainCanvas(ItemExplainCanvas);
            cameraController.SwitchLookControll(true);
        }
    }


    public void OpenMenu()
    {

        playerInput.SwitchCurrentActionMap(uiMapName);
        menuUI.alpha = 1;
        menuUI.interactable = true;
        menuUI.blocksRaycasts = true;
    }

    public void CloseMenu()
    {
        // マップ切替：Player に戻す
        playerInput.SwitchCurrentActionMap(playerMapName);

        menuUI.alpha = 0;
        menuUI.interactable = false;
        menuUI.blocksRaycasts = false;
    }
    private void SwitchExplainCanvas(CanvasGroup canvasToActivate)
    {
        SlotsExplainCanvas.alpha = 0;
        SlotsExplainCanvas.interactable = false;
        SlotsExplainCanvas.blocksRaycasts = false;

        ItemExplainCanvas.alpha = 0;
        ItemExplainCanvas.interactable = false;
        ItemExplainCanvas.blocksRaycasts = false;

        canvasToActivate.alpha = 1;
        canvasToActivate.interactable = true;
        canvasToActivate.blocksRaycasts = true;
    }


}
