using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FpsSurvive.UI
{
    public class EquipDropMe : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Variables
        public Image containerImage;

        public Color highlightColor = Color.green;
        private Color originColor;

        public EquipSlot equipSlot;

        private int invenDropIndex;
        private int weaponDropIndex;
        #endregion

        private void OnEnable()
        {
            originColor = containerImage.color;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (containerImage == null) return;

            containerImage.color = originColor;

            bool isGetValue = GetInvenDropValue(eventData);
            if(isGetValue)
            {
                if(invenDropIndex >= 0)
                {
                    EquipmentEquipItem(invenDropIndex, eventData);
                }

                if(weaponDropIndex >= 0)
                {
                    EquipmentEquipItem(weaponDropIndex, eventData);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(equipSlot == null) return;
            if(containerImage == null) return;

            bool isGetValue = GetInvenDropValue(eventData);
            if(isGetValue)
            {
                containerImage.color = highlightColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (containerImage == null) return;

            containerImage.color = originColor;
        }

        //드랍 아이템 정보 교환
        private bool GetInvenDropValue(PointerEventData eventData)
        {
            bool retValue = false;
            invenDropIndex = -1;
            weaponDropIndex = -1;

            var originObject = eventData.pointerDrag;
            if(originObject == null) return false;

            var invenDrag = originObject.GetComponent<InvenDragMe>();
            if(invenDrag && invenDrag.itemSlot)
            {
                int dropIndex = invenDrag.itemSlot.slotIndex;

                //자리가 유효한지 확인
                if(dropIndex >= 0)
                {
                    int itemType = Equipment.Instance.GetEquipIndex(Inventory.Instance.items[dropIndex].itemId);
                    if (equipSlot.Type == EquipSlotType.MainWeapon && itemType == 0)
                    {
                        invenDropIndex = dropIndex;
                        retValue = true;
                    }
                    else if(equipSlot.Type == EquipSlotType.ConsumWeapon && itemType == 1)
                    {
                        invenDropIndex = dropIndex;
                        retValue = true;
                    }
                    else if(equipSlot.Type == EquipSlotType.Equipment && (itemType >= 2 && itemType <= 5))
                    {
                        invenDropIndex = dropIndex;
                        retValue = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            var equipDrag = originObject.GetComponent<EquipDragMe>();
            if(equipDrag && equipDrag.itemSlot)
            {
                int dropIndex = equipDrag.itemSlot.slotIndex;
                EquipSlot targetSlot = originObject.GetComponent<EquipSlot>();
                if( dropIndex >= 0 )
                {
                    if(equipSlot.Type == targetSlot.Type)
                    {
                        weaponDropIndex = dropIndex;
                        retValue = true;
                    }
                }
            }

            return retValue;
        }

        private void EquipmentEquipItem(int dropIndex, PointerEventData eventData)
        {
            if (dropIndex < 0) return;

            var originObject = eventData.pointerDrag;
            EquipSlot slot = originObject.GetComponent<EquipSlot>();

            if (slot == null)
            {
                Equipment.Instance.EquipItem(Inventory.Instance.items[dropIndex]);
                Inventory.Instance.RemoveItem(Inventory.Instance.items[dropIndex]);
            }
            else
            {
                Equipment.Instance.SwapWeaponSlot(dropIndex, equipSlot.slotIndex, equipSlot.Type);
            }
        }
    }
}