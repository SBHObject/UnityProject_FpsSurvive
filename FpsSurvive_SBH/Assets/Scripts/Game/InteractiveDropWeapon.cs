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
                if ((int)weaponItem.type == 0)
                {
                    Debug.Log("���� �߰� �õ�");
                    isPickup = Equipment.Instance.EquipMainWeapon(new Item(weaponItem));
                }
                else if ((int)weaponItem.type == 1)
                {
                    isPickup = Equipment.Instance.EquipConsumWeapon(new Item(weaponItem));
                }


                if (isPickup == false)
                {
                    Debug.Log("�κ� �߰� �õ�");
                    if ((int)weaponItem.type == 0)
                    {
                        isPickup = Inventory.Instance.AddItem(new Item(weaponItem));
                    }
                    else
                    {
                        int amount = weaponItem.data.creatAmount;
                        isPickup = Inventory.Instance.AddItem(new Item(weaponItem), amount);
                    }
                }
            }

			if (isPickup == true)
            {
                Destroy(gameObject);
            }
        }
    }
}