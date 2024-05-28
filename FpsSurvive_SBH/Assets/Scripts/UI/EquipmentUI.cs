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
		private InventoryUI inventoryUI;
		private ItemInfoUI itemInfoUI;

		public Transform mainWeaponSlotParent;
		public Transform consumWeaponSlotsParent;
		public Transform equipSlotsParent;

		private EquipSlot[] mainWeaponSlots;
		private ItemSlot[] consumWeaponSlots;
		private ItemSlot[] equipSlots;

		public int selectSlotIndex= -1;
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
			inventoryUI = GetComponent<InventoryUI>();
			itemInfoUI = GetComponent<ItemInfoUI>();

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

		public void SelectSlot(int index)
		{
			if(inventoryUI.selectedSlotIndex >= 0)
			{
				inventoryUI.DeSelectSlot();
			}

			if (selectSlotIndex == index)
			{
				DeSelectSlot();
				return;
			}


			selectSlotIndex = index;
            itemInfoUI.OpenItemInfo();
        }

		public void DeSelectSlot()
		{
			if (selectSlotIndex < 0)
				return;

			selectSlotIndex = -1;
			itemInfoUI.CloseItemInfo();
		}

		//창 닫을때 선택 해제
        protected override void UIClose()
        {
            base.UIClose();
			DeSelectSlot();
        }
	}
}