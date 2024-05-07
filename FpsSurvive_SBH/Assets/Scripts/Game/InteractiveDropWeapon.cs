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
                if((int)weaponItem.type == 0)
                {
                    Debug.Log("무기 추가 시도");
                    isPickup = Equipment.Instance.EquipMainWeapon(new Item(weaponItem));
				}
                else if((int)weaponItem.type == 1)
                {
					isPickup = Equipment.Instance.EquipConsumWeapon(new Item(weaponItem));
				}
            }

			if (isPickup == false)
			{
				Debug.Log("인벤 추가 시도");
				isPickup = Inventory.Instance.AddItem(new Item(weaponItem));
			}

			if (isPickup == true)
            {
                Destroy(gameObject);
            }
        }
    }
}