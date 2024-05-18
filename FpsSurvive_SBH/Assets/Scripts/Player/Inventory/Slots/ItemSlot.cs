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

		private void Start()
		{
			equipment = Equipment.Instance;
			inventory = Inventory.Instance;
		}

		public void SetItemSlot(Item newItem, int sIndex)
		{
            slotIndex = sIndex;

			if (newItem.itemId < 0 || newItem == null) return;

			item = newItem;
			iconImage.SetActive(true);
			
			iconImage.GetComponent<Image>().sprite = inventory.itemDatabase.itemObjects[item.itemId].icon;
        }

		public void ResetItemSlot()
		{
			item = null;
			iconImage.SetActive(false);
			iconImage.GetComponent<Image>().sprite = null;
			slotIndex = -1;
		}
	}
}