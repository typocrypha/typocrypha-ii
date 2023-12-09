using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentMenu : MonoBehaviour, IPausable
{
    private const string noBadgeText = "None";
    public bool IsShowing { get; private set; }

    public PauseHandle PH => new PauseHandle();
    private PlayerEquipment Equipment => PlayerDataManager.instance.equipment;

    [SerializeField] private MenuButton first;
    [SerializeField] private GameObject menuObject;
    [SerializeField] private GameObject equipmentNotice;
    [SerializeField] private EquipmentMenuSlot[] slots;

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
        menuObject.SetActive(true);
        equipmentNotice.SetActive(false);
        first.InitializeSelection();
        var equippedBadges = Equipment.EquippedBadgeWords;
        foreach(var slot in slots)
        {
            slot.Button.SetText(equippedBadges.ContainsKey(slot.Slot) ? equippedBadges[slot.Slot].DisplayName : noBadgeText);
        }
        Debug.Log("Showing EquipmentMenu");
    }

    public void Close()
    {
        IsShowing = false;
        menuObject.SetActive(false);
        gameObject.SetActive(false);
        equipmentNotice.SetActive(false);
    }

    private void Update()
    {
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
