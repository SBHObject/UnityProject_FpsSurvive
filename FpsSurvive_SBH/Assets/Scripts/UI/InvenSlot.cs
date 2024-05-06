using FpsSurvive.Game;
using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FpsSurvive.UI
{
    public class InvenSlot : MonoBehaviour
    {
        #region Variables
        private Inventory inventory;
        private InventoryUI inventoryUI;

        private Item item;
        public GameObject iconImageObject;

        private int slotIndex = -1;
        #endregion

        private void Start()
        {
            inventory = Inventory.Instance;
            inventoryUI = GetComponentInParent<InventoryUI>();

            slotIndex = -1;
        }

        public void SetItemSlot(Item newItem, int index)
        {
            item = newItem;
            iconImageObject.SetActive(true);
            iconImageObject.GetComponent<Image>().sprite = inventory.itemDatabase.itemObjects[item.itemId].icon;

            slotIndex = index;
        }

        public void ResetItemSlot()
        {
            item = null;
            iconImageObject.GetComponent<Image>().sprite = null;
            iconImageObject.SetActive(false);
            slotIndex = -1;
        }   
    }
}