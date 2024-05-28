using FpsSurvive.Game;
using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FpsSurvive.UI
{
    public class ShopInfoUI : MonoBehaviour
    {
        #region Variables
        public GameObject shopInfoUI;

        Inventory inventory;
        ShopUI shopUI;

        public ShopManager ShopManager { get; set; }

        private InventoryUI inventoryUI;
        
        //아이템 이름
        public TextMeshProUGUI itemNameText;
        //아이템  설명
        public TextMeshProUGUI itemDescriptionText;
        //아이템 스텟(최대 3개)
        public TextMeshProUGUI statNameTextFirst;
        public TextMeshProUGUI statNameTextSecond;
        public TextMeshProUGUI statNameTextThird;

        //아이템 가격
        public TextMeshProUGUI priceText;

        private string[] statTexts = new string[3];

        public GameObject buyButton;
        public GameObject sellButton;

        //선택 슬롯의 아이템 저장
        private Item item = null;

        public TextMeshProUGUI goldAmountText;
        #endregion

        private void Start()
        {
            inventory = Inventory.Instance;

            inventoryUI = GetComponent<InventoryUI>();
            shopUI = GetComponent<ShopUI>();
        }

        public void SetShopItemInfo()
        {
            UpdateStateInfo();
        }

        public void DeSelectShopItem()
        {
            ResetStateInfo();
        }

        private void UpdateStateInfo()
        {
            ResetStateInfo();

            if(inventoryUI.selectedSlotIndex >= 0)
            {
                item = inventory.items[inventoryUI.selectedSlotIndex];

                //아이템 스텟정보를 가져와 텍스트화
                for(int i = 0; i < item.buffs.Length; i++)
                {
                    if (item.buffs[i] != null)
                    {
                        statTexts[i] = item.buffs[i].stat.ToString() + " : " + item.buffs[i]._value.ToString();
                    }
                    else
                    {
                        statTexts[i] = string.Empty;
                    }
                }

                priceText.text = "Sell Price : " + ShopManager.itemDatabase.itemObjects[item.itemId].sellPrice.ToString();

                sellButton.SetActive(true);
                buyButton.SetActive(false);
            }

            if(shopUI.SelectedSlotIndex >= 0)
            {
                item = ShopManager.shopItems[shopUI.SelectedSlotIndex];

                for(int i = 0; i < item.buffs.Length; i++)
                {
                    if (item.buffs[i] != null)
                    {
                        statTexts[i] = item.buffs[i].stat.ToString() + " : " + item.buffs[i]._value.ToString();
                    }
                    else
                    {
                        statTexts[i] = string.Empty;
                    }
                }

                priceText.text = "Buy Price : " + ShopManager.itemDatabase.itemObjects[item.itemId].buyPrice.ToString();

                sellButton.SetActive(false);
                buyButton.SetActive(true);
            }

            if (item == null) return;

            //아이템 이름, 스텟 정보 세팅
            itemNameText.text = item.itemName;

            itemDescriptionText.text = ShopManager.itemDatabase.itemObjects[item.itemId].description;

            statNameTextFirst.text = statTexts[0];
            statNameTextSecond.text = statTexts[1];
            statNameTextThird.text = statTexts[2];
        }

        public void ResetStateInfo()
        {
            item = null;
            itemNameText.text = string.Empty;
            itemDescriptionText.text= string.Empty;

            for(int i = 0; i < statTexts.Length; i++)
            {
                statTexts[i] = string.Empty;
            }

            statNameTextFirst.text = statTexts[0];
            statNameTextSecond.text = statTexts[1];
            statNameTextThird.text = statTexts[2];
            priceText.text = string.Empty;

            goldAmountText.text = "Gold : " + PlayerStats.Instance.Gold.ToString();

            buyButton.SetActive(false);
            sellButton.SetActive(false);
        }

        public void BuyItem()
        {
            bool isCanBuy = PlayerStats.Instance.UseGold(ShopManager.itemDatabase.itemObjects[item.itemId].buyPrice);

            if (isCanBuy)
            {
                if (item.maxStack == 1)
                {
                    inventory.AddItem(item);
                }
                else
                {
                    inventory.AddItem(item, item.creatAmount);
                }

                shopUI.DeSelectSlot();
            }

            UpdateStateInfo();
        }

        public void SellItem()
        {
            PlayerStats.Instance.AddGold(ShopManager.itemDatabase.itemObjects[item.itemId].sellPrice);

            inventory.RemoveItem(item);
            inventoryUI.DeSelectSlot();
            UpdateStateInfo();
        }
    }
}