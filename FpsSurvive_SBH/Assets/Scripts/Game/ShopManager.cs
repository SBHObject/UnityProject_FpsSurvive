using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Game
{
    public class ShopManager : MonoBehaviour
    {
        #region Variables
        //������ ����
        public ItemDatabase itemDatabase;
        //������ ��ϵ� ������
        public Item[] shopItems = new Item[6];

        private List<ItemObject> weapons = new List<ItemObject>();
        private List<ItemObject> armors = new List<ItemObject>();
        
        //0, 1 ������ ���⸸, 2, 3 ������ ����, 4�� ź��, 5�� ȸ�������� ����
        private const int slotDamperWeapon = 2;
        private const int slotAmmo = 4;
        private const int slotHeal = 5;

        //���� ������ �缳�� Ÿ�̸�
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

            //���� 2�� �������� ���� ���
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

            //���� �����ϸ� ��� �Ҹ�X ������ �߰�X
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