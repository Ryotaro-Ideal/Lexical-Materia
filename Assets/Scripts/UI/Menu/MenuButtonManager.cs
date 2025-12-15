using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour
{
    [Header("Slot")]
    public GameObject ItemSlots;


    [Header("Menu")]
    private List<GameObject> MenuList = new List<GameObject>();
    public GameObject craftMenu;
    public GameObject PlayerInfoMenu;
    public GameObject letterInvMenu;
    public GameObject breakItemMenu;

    public static bool isBreakItemMenuOpen = false;

    [Header("Buttons")]

    public Button craftButton;
    public Button PlayerInfoButton;
    public Button LetterInvButton;

    public Button breakItemButton;

    private void Awake()
    {
        MenuList.Add(craftMenu);
        MenuList.Add(PlayerInfoMenu);
        MenuList.Add(letterInvMenu);
        MenuList.Add(breakItemMenu);
        if (MenuList.Count > 0)
        {
            foreach (var menu in MenuList)
            {
                menu.SetActive(false);
            }
            PlayerInfoMenu.SetActive(true);
        }

    }
    private void Update()
    {
        craftButton.onClick.AddListener(OnCraftButtonClicked);
        PlayerInfoButton.onClick.AddListener(OnPlayerInfoButtonClicked);
        LetterInvButton.onClick.AddListener(OnLetterInvButtonClicked);
        breakItemButton.onClick.AddListener(OnBreakItemButtonClicked);

        if (breakItemMenu.activeSelf)
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
        SwitchMenu(craftMenu);
        SwitchItemSlots(true);
    }
    private void OnPlayerInfoButtonClicked()
    {
        SwitchMenu(PlayerInfoMenu);
        SwitchItemSlots(true);
    }
    private void OnLetterInvButtonClicked()
    {
        SwitchMenu(letterInvMenu);
        SwitchItemSlots(false);

    }
    private void OnBreakItemButtonClicked()
    {
        SwitchMenu(breakItemMenu);
        SwitchItemSlots(true);

    }
    private void SwitchMenu(GameObject menuToActivate)
    {
        foreach (var menu in MenuList)
        {
            menu.SetActive(false);
        }
        menuToActivate.SetActive(true);
    }
    private void SwitchItemSlots(bool state)
    {
        ItemSlots.SetActive(state);
    }
}