using FpsSurvive.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FpsSurvive.Player
{
    public class ItemSlot : MonoBehaviour
    {
		#region Variables
		private Equipment equipment;
		protected Inventory inventory;

		public Item Item { get; private set; }  //슬롯이 가질 아이템
		public GameObject iconImage;			//슬롯에 저장될 아이템의 아이콘

		public int slotIndex = -1;
		private ItemInfoUI itemInfoUI;
		protected InventoryUI inventoryUI;

		//아이템 없을경우 선택 불가능하게 만들 버튼
		protected Button iconButton;
		#endregion

		protected virtual void Start()
		{
			equipment = Equipment.Instance;
			inventory = Inventory.Instance;
			itemInfoUI = GetComponent<ItemInfoUI>();
			inventoryUI = GetComponentInParent<InventoryUI>();
			iconButton = GetComponentInChildren<Button>();

			iconButton.interactable = false;
		}

        public void SetItemSlot(Item newItem, int sIndex)
		{
            slotIndex = sIndex;

			if (newItem == null)
			{
				return;
			}

			if(newItem.itemId < 0)
			{
				return;
			}

			Item = newItem;
			iconImage.SetActive(true);

			if (iconButton != null)
			{
				iconButton.interactable = true;
			}
			iconImage.GetComponent<Image>().sprite = inventory.itemDatabase.itemObjects[Item.itemId].icon;
        }

		public void ResetItemSlot()
		{
			Item = null;
			iconImage.SetActive(false);
			iconImage.GetComponent<Image>().sprite = null;
			slotIndex = -1;
			iconButton.interactable = false;
		}

		public virtual void SelectThisSlot()
		{
			if (Item == null)
				return;

			inventoryUI.SlotSelect(slotIndex);
		}
	}
}