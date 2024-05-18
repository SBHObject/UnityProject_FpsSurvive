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

        public ItemBuffs[] buffs;

        //스택이 가능할경우, 현재 스택 수
        public int amount;
        public int maxStack;
        public int creatAmount;

        //생성자
        public Item()
        {
            itemId = -1;
            itemName = "";
            amount = 0;
            maxStack = 0;
            creatAmount = 0;
        }

        //생성자, ItemObject
        public Item(ItemObject itemObject)
        {
            itemId = itemObject.data.itemId;
            itemName = itemObject.data.itemName;
            amount = (itemObject.maxStack == 0) ? 0 : itemObject.createAmount;
            maxStack = itemObject.maxStack;
            creatAmount = itemObject.createAmount;


            //아이템 능력치 세팅
            buffs = new ItemBuffs[itemObject.data.buffs.Length];
            for (int i = 0; i < itemObject.data.buffs.Length; i++)
            {
                buffs[i] = new ItemBuffs(itemObject.data.buffs[i].Min, itemObject.data.buffs[i].Max)
                {
                    stat = itemObject.data.buffs[i].stat
                };
            }
        }
    }
}