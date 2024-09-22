﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentMenu : MonoBehaviour, IPausable
{
    public const string noBadgeText = "None";
    public bool IsShowing { get; private set; }

    public PauseHandle PH { get; private set; } = null;
    private static PlayerEquipment Equipment => PlayerDataManager.Equipment;

    [SerializeField] private MenuButton first;
    [SerializeField] private GameObject menuObject;
    [SerializeField] private GameObject equipmentNotice;
    [SerializeField] private EquipmentMenuSlot[] slots;
    [SerializeField] private BadgeSelectorMenu badgeSelector;
    [SerializeField] private ShopMenu shopMenu;

    private EquipmentMenuSlot inMenuSlot;
    private bool skipFrame = false;
    public void Enable()
    {
        IsShowing = false;
        menuObject.SetActive(false);
        equipmentNotice.SetActive(true);
        gameObject.SetActive(true);
    }
    public void Open()
    {
        IsShowing = true;
        inMenuSlot = null;
        menuObject.SetActive(true);
        equipmentNotice.SetActive(false);
        first.InitializeSelection();
        foreach(var slot in slots)
        {
            SetSlotText(slot);
        }
        Typocrypha.Keyboard.instance.DisableInactiveSfx = true;
        PauseManager.instance.PauseAll(true, PauseSources.Equipment, PH, true);
    }

    public void Close()
    {
        IsShowing = false;
        menuObject.SetActive(false);
        equipmentNotice.SetActive(true);
        skipFrame = true;
        Typocrypha.Keyboard.instance.DisableInactiveSfx = false;
        PauseManager.instance.PauseAll(false, PauseSources.Equipment, PH, true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void OpenBadgeSelector(int slotIndex)
    {
        inMenuSlot = slots[slotIndex];
        badgeSelector.Open(inMenuSlot.Slot);
    }

    public void CloseBadgeSelector()
    {
        if(inMenuSlot != null)
        {
            inMenuSlot.Button.InitializeSelection();
            SetSlotText(inMenuSlot);
        }
        inMenuSlot = null;
    }

    public void OpenShopMenu()
    {
        PH.Pause(PauseSources.Self);
        shopMenu.OpenLanding();
    }

    public void OnCloseShopMenu()
    {
        PH.Unpause(PauseSources.Self);
    }

    private static void SetSlotText(EquipmentMenuSlot slot)
    {
        var equippedBadges = Equipment.EquippedBadgeWords;
        slot.Button.SetText(equippedBadges.ContainsKey(slot.Slot) ? equippedBadges[slot.Slot].DisplayName : noBadgeText);
    }

    private void Start()
    {
        PH = new PauseHandle();
    }

    private void Update()
    {
        if (skipFrame)
        {
            skipFrame = false;
            return;
        }
        if (PH.Paused)
        {
            return;
        }
        if (IsShowing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (badgeSelector.IsShowing)
                {
                    badgeSelector.CloseNoEquip();
                }
                else
                {
                    Close();
                }
            }
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Open();
        }
    }
}
