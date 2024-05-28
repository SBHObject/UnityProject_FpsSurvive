using FpsSurvive.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    public class ShopSlot : ItemSlot
    {
        private ShopUI shopUI;

        protected override void Start()
        {
            base.Start();

            shopUI = GetComponentInParent<ShopUI>();
        }

        public override void SelectThisSlot()
        {
            if (Item == null) return;

            shopUI.SelectShopSlot(slotIndex);
        }
    }
}