using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI sourceText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;

    public void Setup(BonusEntry entry) 
    {
        nameText.text = entry.badgeName;
        sourceText.text = entry.unlockReason;
        descriptionText.text = entry.description;
        iconImage.sprite = entry.iconSprite;
    }
}
