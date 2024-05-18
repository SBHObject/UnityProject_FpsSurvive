using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Weapon
{
    public class WeaponBulletManager : MonoBehaviour
    {
        #region Variables
        private Inventory inventory;

        public ItemObject itemObject;
        private int currentBullet;
        #endregion

        private void Start()
        {
            currentBullet = 0;
            inventory = Inventory.Instance;
        }

        private void OnEnable()
        {
            GetCurrectBullet();
        }

        public void GetCurrectBullet()
        {
            if(itemObject.type == ItemType.ConsumWeapon)
            {
                currentBullet = inventory.SetCurrentAmmo(itemObject.data.itemId);
            }
        }
    }
}