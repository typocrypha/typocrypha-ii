using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentMenuSlot : MonoBehaviour
{
    public EquipmentWord.EquipmentSlot Slot => slot;
    [SerializeField] private EquipmentWord.EquipmentSlot slot;
    public MenuButton Button => button;
    [SerializeField] private MenuButton button;
}
