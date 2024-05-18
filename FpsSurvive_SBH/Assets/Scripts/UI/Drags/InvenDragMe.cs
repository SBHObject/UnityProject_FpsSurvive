using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FpsSurvive.UI
{
    public class InvenDragMe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Variables
        private Dictionary<int, GameObject> m_DraggingIcons = new Dictionary<int, GameObject>();
        private Dictionary<int, RectTransform> m_DraggingPlanes = new Dictionary<int, RectTransform>();

        public ItemSlot itemSlot;
        #endregion

        public void OnBeginDrag(PointerEventData eventData)
        {
            //�θ� ������Ʈ(�ֿ� ĵ����)
            var canvas = GetComponentInParent<Canvas>();

            if(canvas == null)
            {
                return;
            }

            //������ ������Ʈ ����
            m_DraggingIcons[eventData.pointerId] = new GameObject("Icon");

            //������ ������ ������Ʈ�� �θ� ������Ʈ �Ʒ��� ����
            m_DraggingIcons[eventData.pointerId].transform.SetParent(canvas.transform, false);
            m_DraggingIcons[eventData.pointerId].transform.SetAsLastSibling();

            //������ ������Ʈ �̹���
            var image = m_DraggingIcons[eventData.pointerId].AddComponent<Image>();
            var group = m_DraggingIcons[eventData.pointerId].AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;

            image.sprite = itemSlot.iconImage.GetComponent<Image>().sprite;
            //image.SetNativeSize();

            m_DraggingPlanes[eventData.pointerId] = canvas.transform as RectTransform;
            SetDraggedPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_DraggingIcons[eventData.pointerId] != null)
            {
                SetDraggedPosition(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (m_DraggingIcons[eventData.pointerId] != null)
            {
                Destroy(m_DraggingIcons[eventData.pointerId]);
            }

            m_DraggingIcons[eventData.pointerId] = null;
        }

        private void SetDraggedPosition(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
            {
                m_DraggingPlanes[eventData.pointerId] = eventData.pointerEnter.transform as RectTransform;
            }

            //ĵ���� �� ��ġ
            var rt = m_DraggingIcons[eventData.pointerId].GetComponent<RectTransform>();

            Vector3 globalMousePosition;    //��ġ�� ���
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlanes[eventData.pointerId], eventData.position
                , eventData.pressEventCamera, out globalMousePosition))
            {
                //���� ������ ��ġ ����
                rt.position = globalMousePosition;
                rt.rotation = m_DraggingPlanes[eventData.pointerId].rotation;
            }    
        }
    }
}