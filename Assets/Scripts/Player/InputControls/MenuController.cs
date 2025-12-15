using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    private PlayerInput playerInput;
    private InputHandler inputHandler;
    public GameObject menuUI;



    [Header("マップ名（ActionMap 名）")]
    private string playerMapName = "Player";   // 既存プロジェクトのマップ名に合わせる
    private string uiMapName = "UI";

    private void Awake()
    {

        playerInput = FindFirstObjectByType<PlayerInput>();
        inputHandler = FindFirstObjectByType<InputHandler>();
    }
    private void Update()
    {
        if (inputHandler == null || playerInput == null) return;

        // メニュー開閉入力を監視
        if (inputHandler.IsMenuOpened)
        {
            OpenMenu();
        }
        else if (!inputHandler.IsMenuOpened)
        {
            CloseMenu();
        }
    }


    public void OpenMenu()
    {
        playerInput.SwitchCurrentActionMap(uiMapName);
        menuUI.SetActive(true);
    }

    public void CloseMenu()
    {


        // マップ切替：Player に戻す
        playerInput.SwitchCurrentActionMap(playerMapName);

        menuUI.SetActive(false);
    }


}
