using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour
{
    [Header("Slot")]
    public CanvasGroup ItemSlots;


    [Header("Menu")]
    private List<CanvasGroup> MenuList = new List<CanvasGroup>();
    public CanvasGroup craftMenu;
    public CanvasGroup PlayerInfoMenu;
    public CanvasGroup letterInvMenu;
    public CanvasGroup breakItemMenu;
    public static bool isBreakItemMenuOpen = false;

    [Header("Buttons")]

    public Button craftButton;
    public Button PlayerInfoButton;
    public Button LetterInvButton;

    public Button breakItemButton;


    private void Awake()
    {
        craftButton.onClick.AddListener(OnCraftButtonClicked);
        PlayerInfoButton.onClick.AddListener(OnPlayerInfoButtonClicked);
        LetterInvButton.onClick.AddListener(OnLetterInvButtonClicked);
        breakItemButton.onClick.AddListener(OnBreakItemButtonClicked);

        MenuList.Add(craftMenu);
        MenuList.Add(PlayerInfoMenu);
        MenuList.Add(letterInvMenu);
        MenuList.Add(breakItemMenu);
        if (MenuList.Count > 0)
        {
            foreach (var menu in MenuList)
            {
                menu.alpha = 0;
                menu.interactable = false;
                menu.blocksRaycasts = false;
            }
            PlayerInfoMenu.alpha = 1;
            PlayerInfoMenu.interactable = true;
            PlayerInfoMenu.blocksRaycasts = true;
        }

    }
    private void Update()
    {


        if (breakItemMenu.alpha == 1)
        {
            isBreakItemMenuOpen = true;
        }
        else
        {
            isBreakItemMenuOpen = false;
        }
    }

    private void OnCraftButtonClicked()
    {
        SoundManager.Instance?.PlaySE(SoundName.Click);
        SwitchMenu(craftMenu);
        SwitchItemSlots(true);
    }
    private void OnPlayerInfoButtonClicked()
    {
        SoundManager.Instance?.PlaySE(SoundName.Click);
        SwitchMenu(PlayerInfoMenu);
        SwitchItemSlots(true);
    }
    private void OnLetterInvButtonClicked()
    {
        SoundManager.Instance?.PlaySE(SoundName.Click);
        SwitchMenu(letterInvMenu);
        SwitchItemSlots(false);
    }
    private void OnBreakItemButtonClicked()
    {
        SoundManager.Instance?.PlaySE(SoundName.Click);
        SwitchMenu(breakItemMenu);
        SwitchItemSlots(true);
    }
    private void SwitchMenu(CanvasGroup menuToActivate)
    {
        foreach (var menu in MenuList)
        {
            menu.alpha = 0;
            menu.interactable = false;
            menu.blocksRaycasts = false;
        }
        menuToActivate.alpha = 1;
        menuToActivate.interactable = true;
        menuToActivate.blocksRaycasts = true;
    }
    private void SwitchItemSlots(bool state)
    {
        ItemSlots.alpha = state ? 1 : 0;
        ItemSlots.interactable = state;
        ItemSlots.blocksRaycasts = state;
    }
}