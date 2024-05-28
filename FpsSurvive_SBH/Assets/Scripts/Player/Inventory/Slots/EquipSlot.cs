using FpsSurvive.UI;
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
        private EquipmentUI equipUI;

        public EquipSlotType Type;

        protected override void Start()
        {
            base.Start();
            equipUI = GetComponentInParent<EquipmentUI>();
        }

        public override void SelectThisSlot()
        {
            if(Item == null) return;

            if (inventory.itemDatabase.itemObjects[Item.itemId].type == ItemType.MainWeapon)
            {
                equipUI.SelectSlot(slotIndex);
            }
            else if(inventory.itemDatabase.itemObjects[Item.itemId].type == ItemType.ConsumWeapon)
            {
                equipUI.SelectSlot(slotIndex + 10);
            }
            else
            {
                equipUI.SelectSlot(slotIndex + 20);
            }
        }
    }
}