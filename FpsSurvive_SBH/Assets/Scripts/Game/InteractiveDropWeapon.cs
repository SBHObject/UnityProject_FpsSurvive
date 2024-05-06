using FpsSurvive.Player;
using UnityEngine;

namespace FpsSurvive.Game
{
    public class InteractiveDropWeapon : Interactive
    { 
        #region Variables
        public ItemObject weaponItem;
        #endregion

        public override void DoAction()
        {
            bool isPickup = false;
            if (weaponItem != null && weaponItem.data.itemId > -1)
            {
                Debug.Log("GetItem");
                isPickup = Inventory.Instance.AddItem(new Item(weaponItem));
            }

            if (isPickup == true)
            {
                Destroy(gameObject);
            }
        }
    }
}