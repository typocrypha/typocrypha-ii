using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BadgeSelectorMenu : MonoBehaviour
{
    private static PlayerEquipment Equipment => PlayerDataManager.instance.equipment;

    [SerializeField] private EquipmentMenu parentMenu;
    [SerializeField] private MenuButton[] buttons;
    [SerializeField] private UnityEvent onClose;

    private readonly List<EquipmentWord> unlockedBadges = new List<EquipmentWord>();
    private int selectedBadgeIndex;
    private int selectedButtonIndex;
    private int numActiveButtons;
    private EquipmentWord.EquipmentSlot targetSlot;

    public void Open(EquipmentWord.EquipmentSlot slot)
    {
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

        if (!Equipment.EquippedBadgeWords.ContainsKey(slot) || true)
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
                    break;
                }
            }
        }
        numActiveButtons = -1;
        for (int i = 0; i < buttons.Length; ++i)
        {
            int badgeIndex = selectedBadgeIndex + i;
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
        buttons[0].InitializeSelection();
    }

    public void Close()
    {
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(selectedButtonIndex > 0)
            {
                buttons[--selectedButtonIndex].Select();
                selectedBadgeIndex--;
            }else if(selectedBadgeIndex > -1)
            {
                selectedBadgeIndex--;
                ScrollUp();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedButtonIndex < buttons.Length - 1 && selectedButtonIndex < numActiveButtons - 1)
            {
                buttons[++selectedButtonIndex].Select();
                selectedBadgeIndex++;
            }
            else if(selectedBadgeIndex < unlockedBadges.Count - 1)
            {
                selectedBadgeIndex++;
                ScrollDown();
            }
        }
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

    private static int WordComparer(EquipmentWord w1, EquipmentWord w2)
    {
        return w1.Key.CompareTo(w2.Key);
    }
}
