using FpsSurvive.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Game
{
    public class InteractiveShop : Interactive
    {
        #region Variables
        private ShopManager shopManager;

        private ShopUI shopUi;
        private ShopInfoUI shopInfoUi;
        #endregion

        private void Start()
        {
            shopManager = GetComponent<ShopManager>();
            shopUi = FindObjectOfType<ShopUI>();
            shopInfoUi = FindObjectOfType<ShopInfoUI>();
        }

        public override void DoAction()
        {
            shopUi.ShopManager = shopManager;
            shopInfoUi.ShopManager = shopManager;

            shopUi.ThisUI.SetActive(true);
            shopUi.ShopOpen();
        }
    }
}