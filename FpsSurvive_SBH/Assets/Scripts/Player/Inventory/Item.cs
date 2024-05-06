using FpsSurvive.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    [System.Serializable]
    public class Item
    {
        public int itemId;
        public string itemName;

        //스택이 가능할경우, 현재 스택 수
        public int amount;
        public int maxStack;

        //총알수
        public int createAmount;

        //생성자
        public Item()
        {
            itemId = -1;
            itemName = "";
            amount = 0;
            maxStack = 1;
        }

        //생성자, ItemObject
        public Item(ItemObject itemObject)
        {
            itemId = itemObject.data.itemId;
            itemName = itemObject.data.itemName;
            amount = (itemObject.maxStack == 1) ? 0 : createAmount;
            maxStack = itemObject.maxStack;
        }
    }
}