using FpsSurvive.AnimationParameter;
using FpsSurvive.Game;
using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.UI
{
    public class InventoryUI : ItemUI
    {
		#region Variables
		private Inventory inventory;

		public Transform invenSlotPerant;

		private ItemSlot[] invenSlots;

		private StatsCheckUI infoUI;
		private ItemInfoUI itemInfoUI;
		private ShopInfoUI shopInfoUI;

		private EquipmentUI equipmentUI;
		private ShopUI shopUI;

		public int selectedSlotIndex = -1;

		private bool isShopOpen = false;
		#endregion

		private void Start()
		{
			inventory = Inventory.Instance;
			inventory.OnItemChanged += UpdateInventoryUI;
			invenSlots = invenSlotPerant.GetComponentsInChildren<ItemSlot>();
			equipmentUI = GetComponent<EquipmentUI>();
			itemInfoUI = GetComponent<ItemInfoUI>();
			shopUI = GetComponent<ShopUI>();
			shopInfoUI = GetComponent<ShopInfoUI>();

			infoUI = GetComponent<StatsCheckUI>();

			UpdateInventoryUI();
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.I))
			{
				Toggle();
			}
		}

		public void ShopUIOn()
		{
			isShopOpen = true;
			UIOpen();
		}

		public void ShopUIOff()
		{
			UIClose();
		}

        protected override IEnumerator OpenUIAni()
        {
            isOpen = true;
            playerMove.enabled = false;
            playerNewInput.enabled = false;
            weaponManager.enabled = false;
            ani.SetBool(AniParameters.isOpen, true);

            yield return new WaitForSeconds(animatingTimeLenght);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

			if (isShopOpen == false)
			{
				infoUI.InfoOpen();
			}
			
        }

        protected override IEnumerator CloseUIAni()
        {
			if (isShopOpen == false)
			{
				infoUI.InfoClose();
			}
			else
			{
				isShopOpen = false;
			}
            yield return new WaitForSeconds(animatingTimeLenght);

            isOpen = false;
            ani.SetBool(AniParameters.isOpen, false);

            yield return new WaitForSeconds(animatingTimeLenght);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            playerMove.enabled = true;
            playerNewInput.enabled = true;
            weaponManager.enabled = true;
        }

        void UpdateInventoryUI()
		{
			for(int i = 0; i < invenSlots.Length; i++)
			{
				invenSlots[i].ResetItemSlot();
			}

			for(int i = 0; i < invenSlots.Length; i++)
			{
				if( i < inventory.items.Count)
				{
					invenSlots[i].SetItemSlot(inventory.items[i], i);
				}
			}
		}

		public void SlotSelect(int index)
		{
			if(equipmentUI.selectSlotIndex >= 0 && isShopOpen == false)
			{
				equipmentUI.DeSelectSlot();
			}

			if(shopUI.SelectedSlotIndex >= 0 && isShopOpen == true)
			{
				shopUI.DeSelectSlot();
			}

			if(index == selectedSlotIndex)
			{
				DeSelectSlot();
				return;
			}

			selectedSlotIndex = index;

			if (isShopOpen == false)
			{
				itemInfoUI.OpenItemInfo();
			}
			else
			{
				shopInfoUI.SetShopItemInfo();
			}
		}

		public void DeSelectSlot()
		{
			if (selectedSlotIndex < 0)
				return;

			if (isShopOpen == false)
			{
				itemInfoUI.CloseItemInfo();
			}
			else
			{
				shopInfoUI.DeSelectShopItem();
			}
			selectedSlotIndex = -1;
		}

		//창 닫을때 선택 해제
        protected override void UIClose()
        {
            base.UIClose();
            DeSelectSlot();
        }
    }
}