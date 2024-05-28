using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FpsSurvive.Game;

namespace FpsSurvive.UI
{
    public class ShopItemInfoUI : MonoBehaviour
    {
        #region Variables
        //������ ���� �ؽ�Ʈ�ڽ�
        public TextMeshProUGUI ItemNameText;
        public TextMeshProUGUI ItemDescriptionText;
        public TextMeshProUGUI ItemStatTextFirst;
        public TextMeshProUGUI ItemStatTextSecond;
        public TextMeshProUGUI ItemStatTextThird;

        private ShopUI shopUI;
        private ShopManager shopManager;

        public GameObject buyButton;
        public GameObject sellButton;
        #endregion

        private void Start()
        {
            shopUI = GetComponent<ShopUI>();
            shopManager = shopUI.ShopManager;
        }

    }
}