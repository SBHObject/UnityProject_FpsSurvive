using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    public enum EquipSlotType
    {
        MainWeapon,
        ConsumWeapon,
        Equipment
    }
    public class EquipSlot : ItemSlot
    {
        public EquipSlotType Type;
    }
}