using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Game
{
    public class InteractiveDropEquip : Interactive
    {
        #region Variables
        public ItemObject equipItems;

        private int itemTypeIndex;
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            if(equipItems != null)
            {
                itemTypeIndex = (int)equipItems.type;
            }

            itemTypeIndex -= 2;
        }

        public override void DoAction()
        {
            bool isPickup = false;

            if( equipItems != null &&itemTypeIndex >= 0 && itemTypeIndex <= 3 )
            {
                Equipment.Instance.EquipItem(new Item(equipItems));
                isPickup = true;
            }

            if(isPickup == false)
            {
                Inventory.Instance.AddItem(new Item(equipItems));
            }

            if(isPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}