using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BadgeSelectorMenu : MonoBehaviour
{
    private static PlayerEquipment Equipment => PlayerDataManager.instance.equipment;

    [SerializeField] private EquipmentMenu parentMenu;
    [SerializeField] private MenuButton[] buttons;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private UnityEvent onClose;

    private readonly List<BadgeWord> unlockedBadges = new List<BadgeWord>();
    private int selectedBadgeIndex;
    private int selectedButtonIndex;
    private int numActiveButtons;
    private BadgeWord.EquipmentSlot targetSlot;
    public bool IsShowing { get; private set; } = false;

    public void Open(BadgeWord.EquipmentSlot slot)
    {
        IsShowing = true;
        targetSlot = slot;
        gameObject.SetActive(true);
        unlockedBadges.Clear();
        foreach(var kvp in Equipment.UnlockedBadgeWords)
        {
            if (kvp.Value.Slot != slot)
                continue;
            unlockedBadges.Add(kvp.Value);
        }
        unlockedBadges.Sort(WordComparer);

        int firstBadgeIndex = -1;
        if (!Equipment.EquippedBadgeWords.ContainsKey(slot))
        {
            selectedBadgeIndex = -1;
            selectedButtonIndex = 0;
        }
        else
        {
            var equippedBadge = Equipment.EquippedBadgeWords[slot];
            for (int i = 0; i < unlockedBadges.Count; i++)
            {
                if(equippedBadge == unlockedBadges[i])
                {
                    selectedBadgeIndex = i;
                    if (buttons.Length > selectedBadgeIndex + 1 || buttons.Length > unlockedBadges.Count)
                    {
                        selectedButtonIndex = selectedBadgeIndex + 1;
                    }
                    else if(unlockedBadges.Count - selectedBadgeIndex < buttons.Length)
                    {
                        selectedButtonIndex = buttons.Length - (unlockedBadges.Count - selectedBadgeIndex);
                        firstBadgeIndex = unlockedBadges.Count - buttons.Length;
                    }
                    else
                    {
                        selectedButtonIndex = buttons.Length / 2;
                        firstBadgeIndex = selectedBadgeIndex - selectedButtonIndex;
                    }
                    break;
                }
            }
        }
        numActiveButtons = -1;
        for (int i = 0; i < buttons.Length; ++i)
        {
            int badgeIndex = firstBadgeIndex + i;
            if(badgeIndex < unlockedBadges.Count)
            {
                SetButtonText(buttons[i], badgeIndex);
                buttons[i].gameObject.SetActive(true);
            }
            else
            {
                if(numActiveButtons == -1)
                {
                    numActiveButtons = i;
                }
                buttons[i].gameObject.SetActive(false);
            }
        }
        if(numActiveButtons == -1)
        {
            numActiveButtons = buttons.Length;
        }
        buttons[selectedButtonIndex].InitializeSelection();
        UpdateDescription();
    }

    public void Close()
    {
        IsShowing = false;
        if(selectedBadgeIndex < 0)
        {
            Equipment.UnequipBadgeLive(targetSlot, Battlefield.instance.Player);
        }
        else
        {
            Equipment.EquipBadgeLive(unlockedBadges[selectedBadgeIndex], Battlefield.instance.Player);
        }
        buttons[selectedButtonIndex].OnDeselect(null);
        gameObject.SetActive(false);
        onClose?.Invoke();
    }

    public void CloseNoEquip()
    {
        IsShowing = false;
        buttons[selectedButtonIndex].OnDeselect(null);
        gameObject.SetActive(false);
        onClose?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(selectedButtonIndex > 0)
            {
                buttons[--selectedButtonIndex].Select();
                selectedBadgeIndex--;
                UpdateDescription();
            }
            else if(selectedBadgeIndex > -1)
            {
                selectedBadgeIndex--;
                ScrollUp();
                UpdateDescription();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedButtonIndex < buttons.Length - 1 && selectedButtonIndex < numActiveButtons - 1)
            {
                buttons[++selectedButtonIndex].Select();
                selectedBadgeIndex++;
                UpdateDescription();
            }
            else if(selectedBadgeIndex < unlockedBadges.Count - 1)
            {
                selectedBadgeIndex++;
                ScrollDown();
                UpdateDescription();
            }
        }
    }

    private void UpdateDescription()
    {
        descriptionText.text = selectedBadgeIndex < 0 ? string.Empty : unlockedBadges[selectedBadgeIndex].Description;
    }

    private void SetButtonText(MenuButton button, int badgeIndex)
    {
        button.SetText(badgeIndex < 0 ? EquipmentMenu.noBadgeText : unlockedBadges[badgeIndex].DisplayName);
    }

    private void ScrollUp()
    {
        for (int i = 0; i < buttons.Length; ++i)
        {
            int badgeIndex = selectedBadgeIndex + i;
            SetButtonText(buttons[i], badgeIndex);
        }
        buttons[0].OnSelect(null);
    }



    private void ScrollDown()
    {
        for (int i = 0; i < buttons.Length; ++i)
        {
            int badgeIndex = selectedBadgeIndex - i;
            SetButtonText(buttons[(buttons.Length - 1) - i], badgeIndex);
        }
        buttons[buttons.Length - 1].OnSelect(null);
    }

    private static int WordComparer(BadgeWord w1, BadgeWord w2)
    {
        return w1.Key.CompareTo(w2.Key);
    }
}
