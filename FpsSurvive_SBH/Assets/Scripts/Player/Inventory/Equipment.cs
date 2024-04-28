using FpsSurvive.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

namespace FpsSurvive.Player
{
    public class Equipment : Singleton<Equipment>
    {
		#region Variables
		private Inventory inventory;

		public Item[] mainWeaponItems;
		public Item[] consumWeaponItems;
		public Item[] equipItems;

		public UnityAction<Item, Item> OnEquipChange;

		private const int armorTypeIndex = 2;
		#endregion

		private void Start()
		{
			inventory = Inventory.Instance;

			mainWeaponItems = new Item[3];
			consumWeaponItems = new Item[2];
			equipItems = new Item[4];

			for (int i = 0; i < mainWeaponItems.Length; i++)
			{
				equipItems[i] = new Item();
			}

			for (int i = 0; i < consumWeaponItems.Length; i++)
			{
				equipItems[i] = new Item();
			}

			for (int i = 0; i < equipItems.Length; i++)
			{
				equipItems[i] = new Item();
			}

		}

		//�ֹ���� ����
		public void EquipMainWeapon(Item newWeapon, int equipIndex = -1)
		{
			//0�� Ÿ���� �ƴ� ������ ���� �õ��� ����ó��
			if (GetEquipIndex(newWeapon.itemId) != 0)
			{
				Debug.Log("�߸��� ������ ���� �õ�");
				return;
			}

			Item oldItem = null;
			//�������� ���� ������ ���� �õ���
			if(equipIndex == -1)
			{
				//����ִ� ���� ���Կ� ������ ����, ����ִ� ������ ������� ����ó��
				for(int i = 0; i <mainWeaponItems.Length; i++)
				{
					if(mainWeaponItems[i] == null)
					{
						mainWeaponItems[i] = newWeapon;
						break;
					}
				}
				Debug.Log("�ֹ���ĭ�� ��á���ϴ�.");
			}
			//������ �����ϰ� ������ ���� �õ���
			else
			{
				//���� ������ ���Ⱑ ������� �� ���� ����
				oldItem = mainWeaponItems[equipIndex];
				//���Կ� ���ο� ���� ����
				mainWeaponItems[equipIndex] = newWeapon;

				//���� ���� ���Ⱑ ������� �κ��丮�� �߰�
				if(oldItem != null)
				{
					inventory.AddItem(oldItem, true);
				}
			}
			//���â ���ΰ�ħ
			OnEquipChange?.Invoke(oldItem, newWeapon);
		}

		//�Ҹ� ���� ����
		public void EquipConsumWeapon(Item newItem, int equipIndex = -1)
		{
			if(GetEquipIndex(newItem.itemId) != 1)
			{
				Debug.Log("�߸��� ������ ���� �õ�");
				return;
			}

			Item oldItem = null;
			//�������� ���� ������ ���� �õ���
			if (equipIndex == -1)
			{
				//����ִ� ���� ���Կ� ������ ����, ����ִ� ������ ������� ����ó��
				for (int i = 0; i < mainWeaponItems.Length; i++)
				{
					if (mainWeaponItems[i] == null)
					{
						mainWeaponItems[i] = newItem;
						break;
					}
				}
				Debug.Log("�Ҹ� ����ĭ�� ��á���ϴ�.");
			}
			//������ �����ϰ� ������ ���� �õ���
			else
			{
				//���� ������ ���Ⱑ ������� �� ���� ����
				oldItem = mainWeaponItems[equipIndex];
				//���Կ� ���ο� ���� ����
				mainWeaponItems[equipIndex] = newItem;

				//���� ���� ���Ⱑ ������� �κ��丮�� �߰�
				if (oldItem != null)
				{
					inventory.AddItem(oldItem, oldItem.createAmount, true);
				}
			}
			//���â ���ΰ�ħ
			OnEquipChange?.Invoke(oldItem, newItem);

		}

		//�� ����
		public void EquipItem(Item newItem)
		{
			//������ �ε��� ���� (�� �迭 Ÿ�� 2 ~ 5, ���� 0 ~ 3���� ����)
			int index = GetEquipIndex(newItem.itemId) - armorTypeIndex;
			//�������� �������� ����(������� null ����)
			Item oldItem = equipItems[index];
			//���������ۿ� ���� �õ��� ������ ����
			equipItems[index] = newItem;
			//�������� �������� �־������, �ش� ������ �κ��丮�� �߰�
			if(oldItem != null && oldItem.itemId >= armorTypeIndex)
			{
				inventory.AddItem(oldItem , true);
			}
			//UI ���ΰ�ħ
			OnEquipChange?.Invoke(oldItem, newItem);
		}

		public int GetEquipIndex(int itemId)
		{
			ItemType itemType = inventory.itemDatabase.itemObjects[itemId].type;

			return (int)itemType;
		}
	}
}