﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentMenu : MonoBehaviour, IPausable
{
    public const string noBadgeText = "None";
    public bool IsShowing { get; private set; }

    public PauseHandle PH => new PauseHandle();
    private static PlayerEquipment Equipment => PlayerDataManager.instance.equipment;

    [SerializeField] private MenuButton first;
    [SerializeField] private GameObject menuObject;
    [SerializeField] private GameObject equipmentNotice;
    [SerializeField] private EquipmentMenuSlot[] slots;
    [SerializeField] private BadgeSelectorMenu badgeSelector;

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
        Debug.Log("Showing EquipmentMenu");
    }

    public void Close()
    {
        IsShowing = false;
        menuObject.SetActive(false);
        equipmentNotice.SetActive(true);
        skipFrame = true;
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

    private static void SetSlotText(EquipmentMenuSlot slot)
    {
        var equippedBadges = Equipment.EquippedBadgeWords;
        slot.Button.SetText(equippedBadges.ContainsKey(slot.Slot) ? equippedBadges[slot.Slot].DisplayName : noBadgeText);
    }

    private void Update()
    {
        if (skipFrame)
        {
            skipFrame = false;
            return;
        }
        if (PH.Pause)
        {
            return;
        }
        if (IsShowing)
        {
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Open();
        }
    }
}
