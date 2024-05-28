using FpsSurvive.AnimationParameter;
using FpsSurvive.Game;
using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.UI
{
    public class ShopUI : ItemUI
    {
        #region Variables
        //상점 스크립트
        public ShopManager ShopManager { get; set; }

        //아이템 슬롯
        private ShopSlot[] shopRandomSlots;
        public int RandomShopSlotLength() => shopRandomSlots.Length;
        public Transform shopRandomSlotsParent;

        private ShopSlot[] shopSlots;
        public int ShopSlotLength() => shopSlots.Length;
        public Transform shopSlotsParent;

        //인벤토리 같이 열기위한 준비
        private InventoryUI invenUI;
        private ShopInfoUI shopInfoUI;

        public bool IsShopOpen { get; private set; }

        public int SelectedSlotIndex { get; private set; }
        #endregion

        private void Start()
        {
            shopRandomSlots = shopRandomSlotsParent.GetComponentsInChildren<ShopSlot>();
            shopSlots = shopSlotsParent.GetComponentsInChildren<ShopSlot>();
            invenUI = GetComponent<InventoryUI>();
            shopInfoUI = GetComponent<ShopInfoUI>();

            SelectedSlotIndex = -1;

            ThisUI.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIClose();
            }
        }

        public void ShopOpen()
        {
            IsShopOpen = true;
            SetShopUI();
            shopInfoUI.ResetStateInfo();
        }

        protected override IEnumerator CloseUIAni()
        {
            isOpen = false;
            ani.SetBool(AniParameters.isOpen, false);
            invenUI.ShopUIOff();
            yield return new WaitForSeconds(animatingTimeLenght);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            playerMove.enabled = true;
            playerNewInput.enabled = true;
            weaponManager.enabled = true;

            ResetShopUI();

            ShopManager = null;
            IsShopOpen = false;
            ThisUI.SetActive(false);
        }

        private void SetShopUI()
        {
            for (int i = 0; i < shopRandomSlots.Length; i++)
            {
                shopRandomSlots[i].SetItemSlot(ShopManager.shopItems[i], i);
            }

            for(int i = 0; i < shopSlots.Length; i++)
            {
                shopSlots[i].SetItemSlot(ShopManager.shopItems[i + shopRandomSlots.Length], i + shopRandomSlots.Length);
            }

            UIOpen();
            invenUI.ShopUIOn();
        }

        private void ResetShopUI()
        {
            for (int i = 0; i < shopSlots.Length; i++)
            {
                shopSlots[i].ResetItemSlot();
            }
        }

        public void SelectShopSlot(int index)
        {
            if(invenUI.selectedSlotIndex >= 0)
            {
                invenUI.DeSelectSlot();
            }

            if(index == SelectedSlotIndex)
            {
                return;
            }

            SelectedSlotIndex = index;
            
            shopInfoUI.SetShopItemInfo();
        }

        public void DeSelectSlot()
        {
            if (SelectedSlotIndex < 0)
                return;

            shopInfoUI.DeSelectShopItem();
            SelectedSlotIndex = -1;
        }

    }
}