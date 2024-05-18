using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FpsSurvive.UI
{
    public class InvenDropMe : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Variables
        public Image containerImage;

        public Color highlightColor = Color.green;
        private Color originColor;

        public ItemSlot m_ItemSlot;

        private int invenDropIndex = -1;
        private int equipDropIndex = -1;
        #endregion

        private void OnEnable()
        {
            if(containerImage != null)
            {
                originColor = containerImage.color;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (containerImage == null)
                return;

            containerImage.color = originColor;

            //�������� �� ���Կ� ����������
            bool isGetValue = GetInvenDropIndex(eventData);
            if(isGetValue)
            {
                //�κ��丮���� ��������ġ�� �ٲܰ��
                if(invenDropIndex >= 0)
                {
                    InvenSwapItems(invenDropIndex);
                }

                //���� �����ۿ� ����Ұ��
                if(equipDropIndex >= 0)
                {
                    EquipSwapItem(invenDropIndex, eventData);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(m_ItemSlot == null)
            {
                return;
            }

            if (containerImage == null)
                return;

            //�ٲܼ� �������
            bool isGetValue = GetInvenDropIndex(eventData);
            if(isGetValue)
            {
                containerImage.color = highlightColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(containerImage == null) return;

            containerImage.color = originColor;
        }

        //�巡������ �������� ���� ��������
        private bool GetInvenDropIndex(PointerEventData eventData)
        {
            //������ �ʱ�ȭ
            bool retValue = false;
            invenDropIndex = -1;
            equipDropIndex = -1;

            var originalObject = eventData.pointerDrag;
            //�κ��丮 
            if(originalObject == null)
            {
                return false;
            }

            var invenDrag = originalObject.GetComponent<InvenDragMe>();
            if(invenDrag == true && invenDrag.itemSlot && m_ItemSlot.slotIndex >= 0)
            {
                invenDropIndex = invenDrag.itemSlot.slotIndex;
                retValue = true;
            }

            var equipDrag = originalObject.GetComponent<EquipDragMe>();
            if(equipDrag && equipDrag.itemSlot && m_ItemSlot.slotIndex >= 0)
            {
                equipDropIndex = equipDrag.itemSlot.slotIndex;
                retValue = true;
            }
            
            return retValue;
        }

        private void InvenSwapItems(int dropIndex)
        {
            if (dropIndex < 0)
                return;

            if (m_ItemSlot == null || m_ItemSlot.slotIndex < 0)
                return;

            Inventory.Instance.SwapItem(dropIndex, m_ItemSlot.slotIndex);
        }

        private void EquipSwapItem(int dropIndex, PointerEventData eventData)
        {
            if (dropIndex < 0)
                return;

            var originPointer = eventData.pointerDrag;
            EquipSlot equipSlot = originPointer.GetComponent<EquipSlot>();

            Equipment.Instance.UnequipItems(dropIndex, equipSlot.Type);
        }
    }
}