using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FpsSurvive.Utility;
using UnityEngine.Events;

namespace FpsSurvive.Player 
{
    public class Inventory : Singleton<Inventory>
    {
		#region Variables
		//���� ��ü ������
		public ItemDatabase itemDatabase;

        //�κ��丮�� ������ ���
        public List<Item> items = new List<Item>();

		//�⺻ �κ��丮 ũ��
		[SerializeField]
		private int baseInvenSize = 6;
		private const int unstackableItemStack = 1;

		public int FinalInvenSize { get; set; }
		
		public UnityAction OnItemChanged;
		#endregion

		protected override void Awake()
		{
			base.Awake();

			FinalInvenSize = baseInvenSize;
		}

		//�κ��丮 ��ĭã��
		public bool IsEmpty(Item newItem)
		{
			bool empty = items.Count < FinalInvenSize;

			//���� ������ �������ϰ��
			if (itemDatabase.itemObjects[newItem.itemId].maxStack > unstackableItemStack)
			{
				foreach(Item item in items)
				{
					if(newItem.itemId == item.itemId)
					{
						if(item.amount < item.maxStack)
						{
							empty = true;
							break;
						}
					}
				}
			}

			return empty;
		}

		//�κ��丮 ������ �߰�
		public bool AddItem(Item newitem, bool exchange = false)
		{
			if((IsEmpty(newitem) == false && exchange == false) || itemDatabase.itemObjects[newitem.itemId].maxStack > unstackableItemStack)
			{
				Debug.Log("�κ��丮 ���� ���� or �߸��� ��û");
				return false;
			}

			items.Add(newitem);

			OnItemChanged?.Invoke();
			return true;
		}

		//�Ѿ� ȹ��ÿ� ���(�ѹ��� �ټ��� ���� ȹ��)
		public bool AddItem(Item newItem, int amount, bool exchange = false)
		{
			if((IsEmpty(newItem) == false && exchange == false) || itemDatabase.itemObjects[newItem.itemId].maxStack == unstackableItemStack)
			{
				Debug.Log("�߰� ����, ������ �����ϰų� �߸��� ��û�Դϴ�(�� ������ ������ ���þ����� ȹ�� ��û)");
				return false;
			}

			//���� ������ ���� ���� üũ
			bool isFind = false;
			foreach(Item item in items)
			{
				if(item.itemId == newItem.itemId)
				{
					if(item.amount < item.maxStack)
					{
						isFind = true;
						if(item.amount + amount > item.maxStack)
						{
							item.amount = item.maxStack;
							//�ϴ� �������� �Ҹ�, ���� ȹ��� ��ȯ�ϵ��� �����غ���
						}
						break;
					}
				}
			}

			//���� �������� ������� �κ��丮�� �߰�
			if(isFind == false)
			{
				items.Add(newItem);
			}

			OnItemChanged?.Invoke();
			return true;
		}

		//������ ������ ����
		public void RemoveItem(Item oldItem)
		{
			if (itemDatabase.itemObjects[oldItem.itemId].maxStack > unstackableItemStack)
			{
				Debug.Log("�߸��� ��û�Դϴ�(������ �������ε� ������ ������ ���� ��û �õ�)");
			}

			items.Remove(oldItem);

			OnItemChanged?.Invoke();
		}

		public void RemoveItem(Item oldItem, int amount)
		{
			if(itemDatabase.itemObjects[oldItem.itemId].maxStack == unstackableItemStack)
			{
				Debug.Log("�߸��� ��û�Դϴ�(������ �������ε� ������ ������ ���� ��û �õ�)");
			}

			//������ �ִ�ġ �Է��� ���� ���������� �����Ұ�...
			oldItem.amount -= amount;
			if(oldItem.amount <= 0)
			{
				items.Remove(oldItem);
			}

			OnItemChanged?.Invoke();
		}

		//������ ��ġ ����
		public void SwapItem(int indexA, int indexB)
		{
			if(indexA == indexB)
					return;

			Item temp = items[indexB];
			items[indexB] = items[indexA];
			items[indexA] = temp;

			OnItemChanged?.Invoke();
		}

		//�������� üũ
		public bool IsEquipItem(int itemId)
		{
			ItemType itemType = itemDatabase.itemObjects[itemId].type;
			switch(itemType)
			{
				case ItemType.MainWeapon:
				case ItemType.ConsumWeapon:
				case ItemType.Helmet:
				case ItemType.Armor:
				case ItemType.Boots:
				case ItemType.Backpack:
					return true;
			}
			return false;
		}
	}
}