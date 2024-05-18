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
            //부모 오브젝트(주요 캔버스)
            var canvas = GetComponentInParent<Canvas>();

            if(canvas == null)
            {
                return;
            }

            //아이콘 오브젝트 생성
            m_DraggingIcons[eventData.pointerId] = new GameObject("Icon");

            //생성한 아이콘 오브젝트를 부모 오브젝트 아래에 넣음
            m_DraggingIcons[eventData.pointerId].transform.SetParent(canvas.transform, false);
            m_DraggingIcons[eventData.pointerId].transform.SetAsLastSibling();

            //아이콘 오브젝트 이미지
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

            //캔버스 내 위치
            var rt = m_DraggingIcons[eventData.pointerId].GetComponent<RectTransform>();

            Vector3 globalMousePosition;    //위치값 출력
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlanes[eventData.pointerId], eventData.position
                , eventData.pressEventCamera, out globalMousePosition))
            {
                //실제 아이콘 위치 지정
                rt.position = globalMousePosition;
                rt.rotation = m_DraggingPlanes[eventData.pointerId].rotation;
            }    
        }
    }
}