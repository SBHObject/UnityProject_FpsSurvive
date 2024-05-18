using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FpsSurvive.Game
{
    public class InteractiveDropItem : Interactive
    {
        #region Variables
        public ItemObject itemObject;
        #endregion

        public override void DoAction()
        {
            bool isPickup = false;

            if(itemObject != null)
            {
                Inventory.Instance.AddItem(new Item(itemObject), itemObject.createAmount);
                isPickup = true;
            }

            if(isPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}