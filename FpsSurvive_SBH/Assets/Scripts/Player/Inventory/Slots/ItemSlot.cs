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
		private Inventory inventory;

		private Item item;  //슬롯이 가질 아이템
		public GameObject iconImage;

		public int slotIndex = -1;
		#endregion

		private void OnEnable()
		{
			if(equipment == null)
				equipment = Equipment.Instance;
			if(inventory == null)
				inventory = Inventory.Instance;
		}

		public void SetItemSlot(Item newItem, int sIndex)
		{
			slotIndex = sIndex;

			if (newItem == null || newItem.itemId < 0)
				return;

			item = newItem;
			iconImage.SetActive(true);
			iconImage.GetComponent<Image>().sprite = inventory.itemDatabase.itemObjects[item.itemId].icon;
		}

		public void ResetItemSlot()
		{
			item = null;
			iconImage.SetActive(false);
			iconImage.GetComponent<Image>().sprite = null;
		}
	}
}