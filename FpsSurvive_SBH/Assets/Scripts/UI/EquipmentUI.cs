using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FpsSurvive.UI
{
    public class EquipmentUI : ItemUI
    {
		#region Variables
		private Equipment equip;

		public Transform mainWeaponSlotParent;
		public Transform consumWeaponSlotsParent;
		public Transform equipSlotsParent;

		private EquipSlot[] mainWeaponSlots;
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
			mainWeaponSlots = mainWeaponSlotParent.GetComponentsInChildren<EquipSlot>();
			consumWeaponSlots = consumWeaponSlotsParent.GetComponentsInChildren<ItemSlot>();
			equipSlots = equipSlotsParent.GetComponentsInChildren<ItemSlot>();

			equip.OnEquipChange += UpdateEquipUI;

			UpdateEquipUI(null, null);
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.I))
			{
				Toggle();
			}
		}

		void UpdateEquipUI(Item oldItem, Item newItem)
		{
			//UI 초기화
			for (int i = 0; i < mainWeaponSlots.Length; i++)
			{
				mainWeaponSlots[i].ResetItemSlot();
			}

			for (int i = 0; i < consumWeaponSlots.Length; i++)
			{
				consumWeaponSlots[i].ResetItemSlot();
			}

			for (int i = 0; i < equipSlots.Length; i++)
			{
				equipSlots[i].ResetItemSlot();
			}

			//UI 입력
			for (int i = 0; i < mainWeaponSlots.Length; i++)
			{
				mainWeaponSlots[i].SetItemSlot(equip.mainWeaponItems[i], i);
			}
			for (int i = 0; i < consumWeaponSlots.Length; i++)
			{
				consumWeaponSlots[i].SetItemSlot(equip.consumWeaponItems[i], i);
			}

			for (int i = 0; i < equipSlots.Length; i++)
			{
				equipSlots[i].SetItemSlot(equip.equipItems[i], i);
			}
		}
	}
}