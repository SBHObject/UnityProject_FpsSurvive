using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.UI
{
    public class EquipmentUI : ItemUI
    {
		#region Variables
		private Equipment equip;

		public Transform mainWeaponSlotParent;
		public Transform consumWeaponSlotsParent;
		public Transform equipSlotsParent;

		private ItemSlot[] mainWeaponSlots;
		private ItemSlot[] consumWeaponSlots;
		private ItemSlot[] equipSlots;
		#endregion

		protected override void Awake()
		{
			base.Awake();
			equip = Equipment.Instance;
		}

		private void Start()
		{
			mainWeaponSlots = mainWeaponSlotParent.GetComponentsInChildren<ItemSlot>();
			consumWeaponSlots = consumWeaponSlotsParent.GetComponentsInChildren<ItemSlot>();
			equipSlots = equipSlotsParent.GetComponentsInChildren<ItemSlot>();
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.I))
			{
				Toggle();
			}
		}
	}
}