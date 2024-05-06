using FpsSurvive.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item")]
    public class ItemObject : ScriptableObject
    {
        public ItemType type;
        public int maxStack;

        public Sprite icon;

        public Item data = new Item();

        [TextArea(15, 20)]
        public string description;

        public int buyPrice;
        public int sellPrice;

        public WeaponController weaponPrefab;
        public Item CreateItem()
        {
            Item newItem = new Item(this);
            return newItem;
        }
    }
}