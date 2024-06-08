using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentMenuSlot : MonoBehaviour
{
    public BadgeWord.EquipmentSlot Slot => slot;
    [SerializeField] private BadgeWord.EquipmentSlot slot;
    public MenuButton Button => button;
    [SerializeField] private MenuButton button;
}
