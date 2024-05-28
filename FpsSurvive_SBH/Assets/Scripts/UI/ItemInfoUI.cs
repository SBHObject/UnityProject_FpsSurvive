using FpsSurvive.AnimationParameter;
using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FpsSurvive.UI
{
    public class ItemInfoUI : ItemUI
    {
        #region Variables
        Inventory inventory;
        Equipment equipment;

        private InventoryUI inventoryUI;
        private EquipmentUI equipmentUI;

        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI statNameTextFirst;
        public TextMeshProUGUI statNameTextSecond;
        public TextMeshProUGUI statNameTextThird;

        private string[] statTexts = new string[3];

        public GameObject equipButton;
        public GameObject unEquipButton;

        //선택 슬롯의 아이템 저장
        private Item item = null;
        #endregion

        private void Start()
        {
            inventory = Inventory.Instance;
            equipment = Equipment.Instance;

            inventoryUI = GetComponent<InventoryUI>();
            equipmentUI = GetComponent<EquipmentUI>();
        }

        public void OpenItemInfo()
        {
            ThisUI.SetActive(true);
            
            UpdateStats();
            UIOpen();
        }

        public void CloseItemInfo()
        {
            StatTextReset();
            UIClose();

            ThisUI.SetActive(false);
        }

        private void UpdateStats()
        {
            StatTextReset();

            //인벤토리 선택시, 선택한 슬롯의 아이템 정보 가져오기
            if (inventoryUI.selectedSlotIndex >= 0)
            {
                item = inventory.items[inventoryUI.selectedSlotIndex];

                for(int i = 0; i < item.buffs.Length; i++)
                {
                    if (item.buffs[i] != null)
                    {
                        statTexts[i] = item.buffs[i].stat.ToString() + " : " + item.buffs[i]._value.ToString();
                    }
                    else
                    {
                        statTexts[i] = "";
                    }
                }

                equipButton.SetActive(true);
                unEquipButton.SetActive(false);
            }
            
            //장비창 선택시
            if(equipmentUI.selectSlotIndex >= 0)
            {
                if (equipmentUI.selectSlotIndex < 10)
                {
                    item = equipment.mainWeaponItems[equipmentUI.selectSlotIndex];
                    for (int i = 0; i < item.buffs.Length; i++)
                    {
                        if (item.buffs[i] != null)
                        {
                            statTexts[i] = item.buffs[i].stat.ToString() + " : " + item.buffs[i]._value.ToString();
                        }
                        else
                        {
                            statTexts[i] = "";
                        }
                    }
                }
                else if (equipmentUI.selectSlotIndex < 20)
                {
                    item = equipment.consumWeaponItems[equipmentUI.selectSlotIndex - 10];
                    for (int i = 0; i < item.buffs.Length; i++)
                    {
                        if (item.buffs[i] != null)
                        {
                            statTexts[i] = item.buffs[i].stat.ToString() + " : " + item.buffs[i]._value.ToString();
                        }
                        else
                        {
                            statTexts[i] = "";
                        }
                    }
                }
                else
                {
                    item = equipment.equipItems[equipmentUI.selectSlotIndex - 20];
                    for (int i = 0; i < item.buffs.Length; i++)
                    {
                        if (item.buffs[i] != null)
                        {
                            statTexts[i] = item.buffs[i].stat.ToString() + " : " + item.buffs[i]._value.ToString();
                        }
                        else
                        {
                            statTexts[i] = "";
                        }
                    }
                }

                equipButton.SetActive(false);
                unEquipButton.SetActive(true);
            }

            if (item == null) return;

            //아이템 이름, 스텟 정보 세팅
            itemNameText.text = item.itemName;
            statNameTextFirst.text = statTexts[0];
            statNameTextSecond.text = statTexts[1];
            statNameTextThird.text = statTexts[2];
        }

        private void StatTextReset()
        {
            item = null;
            itemNameText.text = "";
            for(int i = 0; i < statTexts.Length;i++)
            {
                statTexts[i] = "";
            }

            statNameTextFirst.text = statTexts[0];
            statNameTextSecond.text = statTexts[1];
            statNameTextThird.text = statTexts[2];

            equipButton.SetActive(false);
            unEquipButton.SetActive(false);
        }

        protected override IEnumerator CloseUIAni()
        {
            isOpen = false;
            ani.SetBool(AniParameters.isOpen, false);

            yield return new WaitForSeconds(animatingTimeLenght);
        }

        public void PressEquipButton()
        {
            if (equipmentUI.selectSlotIndex >= 0 || item == null) return;

            if (inventory.itemDatabase.itemObjects[item.itemId].type == ItemType.MainWeapon)
            {
                equipment.EquipMainWeapon(item);
                inventory.RemoveItem(item);
            }
            else if (inventory.itemDatabase.itemObjects[item.itemId].type == ItemType.ConsumWeapon)
            {
                equipment.EquipConsumWeapon(item);
                inventory.RemoveItem(item, item.amount);
            }
            else
            {
                equipment.EquipItem(item);
                inventory.RemoveItem(item);
            }

            inventoryUI.DeSelectSlot();
        }

        public void PressUnEquipButton()
        {
            if(inventoryUI.selectedSlotIndex >= 0 || item == null) return;

            if(equipmentUI.selectSlotIndex < 10)
            {
                equipment.UnequipItems(equipmentUI.selectSlotIndex, EquipSlotType.MainWeapon);
            }
            else if (equipmentUI.selectSlotIndex < 20)
            {
                equipment.UnequipItems(equipmentUI.selectSlotIndex - 10, EquipSlotType.ConsumWeapon);
            }
            else
            {
                equipment.UnequipItems(equipmentUI.selectSlotIndex - 20, EquipSlotType.Equipment);
            }

            equipmentUI.DeSelectSlot();
        }

        public void PressDropButton()
        {

        }
    }
}