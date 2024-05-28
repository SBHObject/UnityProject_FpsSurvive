using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Game
{
    public class ShopManager : MonoBehaviour
    {
        #region Variables
        //아이템 정보
        public ItemDatabase itemDatabase;
        //상점에 등록될 아이템
        public Item[] shopItems = new Item[6];

        private List<ItemObject> weapons = new List<ItemObject>();
        private List<ItemObject> armors = new List<ItemObject>();
        
        //0, 1 번슬롯 무기만, 2, 3 번슬롯 방어구만, 4번 탄약, 5번 회복아이템 고정
        private const int slotDamperWeapon = 2;
        private const int slotAmmo = 4;
        private const int slotHeal = 5;

        //상점 아이템 재설정 타이머
        private float lastShopResetTime;
        [SerializeField]
        private float shopResetTime = 300f;
        #endregion

        private void Start()
        {
            foreach (ItemObject itemObject in itemDatabase.itemObjects)
            {
                if (itemObject.type == ItemType.MainWeapon || itemObject.type == ItemType.ConsumWeapon)
                {
                    weapons.Add(itemObject);
                }

                if(itemObject.type == ItemType.Helmet || itemObject.type == ItemType.Boots || itemObject.type == ItemType.Backpack
                    || itemObject.type == ItemType.Armor)
                {
                    armors.Add(itemObject);
                }
            }

            SetShopItems();
        }

        private void Update()
        {
            if(lastShopResetTime + shopResetTime <= Time.time)
            {
                SetShopItems();
            }
        }

        private void SetShopItems()
        {
            for(int i = 0; i < shopItems.Length; i++)
            {
                shopItems[i] = null;
            }

            //무기 2종 랜덤으로 상점 등록
            for(int i = 0; i < slotDamperWeapon; i++)
            {
                int randomItem = Random.Range(0, weapons.Count);

                shopItems[i] = new Item(weapons[randomItem]);
            }

            for(int i = slotDamperWeapon; i < slotAmmo; i++)
            {
                int randomItem = Random.Range(0, armors.Count);

                shopItems[i] = new Item(armors[randomItem]);
            }

            shopItems[slotAmmo] = new Item(itemDatabase.itemObjects[6]);
            shopItems[slotHeal] = new Item(itemDatabase.itemObjects[7]);

            lastShopResetTime = Time.time;
        }

        public void SoldShopItem(int index, int amount = -1)
        {
            if (shopItems[index] == null) return;

            //돈이 부족하면 골드 소모X 아이템 추가X
            if (PlayerStats.Instance.UseGold(itemDatabase.itemObjects[shopItems[index].itemId].buyPrice) == false)
            {
                return;
            }

            if (shopItems[index].maxStack == 1)
            {
                Inventory.Instance.AddItem(shopItems[index]);

                shopItems[index] = null;
            }
            else
            {
                if (amount < 0) return;
                
                if(amount > shopItems[index].amount)
                {
                    Inventory.Instance.AddItem(shopItems[index]);
                    amount = shopItems[index].amount;
                }

                shopItems[index].amount -= amount;
                if (shopItems[index].amount <= 0)
                {
                    Inventory.Instance.AddItem(shopItems[index]);
                    shopItems[index] = null;
                }
            }
        }

        public void SellItem(Item item)
        {
            int finalPrice = (int)(Inventory.Instance.itemDatabase.itemObjects[item.itemId].sellPrice * PlayerStats.Instance.GoldGainIncrese);

            PlayerStats.Instance.AddGold(finalPrice);
        }
    }
}