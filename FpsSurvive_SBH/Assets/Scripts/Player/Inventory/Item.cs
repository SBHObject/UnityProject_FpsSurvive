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

        //������ �����Ұ��, ���� ���� ��
        public int amount;
        public int maxStack;

        //�Ѿ˼�
        public int createAmount;

        //������
        public Item()
        {
            itemId = -1;
            itemName = "";
            amount = 0;
            maxStack = 1;
        }

        //������, ItemObject
        public Item(ItemObject itemObject)
        {
            itemId = itemObject.data.itemId;
            itemName = itemObject.data.itemName;
            amount = (itemObject.maxStack == 1) ? 0 : createAmount;
            maxStack = itemObject.maxStack;

            //������ �ɷ�ġ ����
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