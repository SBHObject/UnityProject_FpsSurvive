using FpsSurvive.Utility;
using FpsSurvive.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FpsSurvive.Player
{
    public class Equipment : Singleton<Equipment>
    {
		#region Variables
		private Inventory inventory;
		private PlayerWeaponManager weaponManager;

		public Item[] mainWeaponItems;
		public Item[] consumWeaponItems;
		public Item[] equipItems;

		public UnityAction<Item, Item> OnEquipChange;

		private const int armorTypeIndex = 2;
		private const int consumedWeaponIndex = 3;

		#endregion

		private void Start()
		{
			inventory = Inventory.Instance;
			weaponManager = FindObjectOfType<PlayerWeaponManager>();

			mainWeaponItems = new Item[3];
			consumWeaponItems = new Item[2];
			equipItems = new Item[4];

			for (int i = 0; i < mainWeaponItems.Length; i++)
			{
				mainWeaponItems[i] = new Item();
			}

			for (int i = 0; i < consumWeaponItems.Length; i++)
			{
				consumWeaponItems[i] = new Item();
			}

			for (int i = 0; i < equipItems.Length; i++)
			{
				equipItems[i] = new Item();
			}

		}

		//�ֹ���� ����
		public bool EquipMainWeapon(Item newWeapon, int equipIndex = -1)
		{
			//0�� Ÿ���� �ƴ� ������ ���� �õ��� ����ó��
			if (GetEquipIndex(newWeapon.itemId) != 0)
			{
				Debug.Log("�߸��� ������ ���� �õ�");
				return false;
			}

			bool isEquiped = false;

			Item oldItem = null;

			//�������� ���� ������ ���� �õ���
			if(equipIndex == -1)
			{
				//����ִ� ���� ���Կ� ������ ����, ����ִ� ������ ������� ����ó��
				for(int i = 0; i <mainWeaponItems.Length; i++)
				{
					if(mainWeaponItems[i].itemId == -1)
					{
						Debug.Log("��ĭ");
						mainWeaponItems[i] = newWeapon;
						weaponManager.AddWeapon(inventory.itemDatabase.itemObjects[newWeapon.itemId].weaponPrefab);
						isEquiped = true;
						break;
					}
					Debug.Log("�ֹ���ĭ�� ��á���ϴ�.");
				}
			}
			//������ �����ϰ� ������ ���� �õ���
			else
			{
				//���� ������ ���Ⱑ ������� �� ���� ����
				oldItem = mainWeaponItems[equipIndex];
				//���Կ� ���ο� ���� ����
				mainWeaponItems[equipIndex] = newWeapon;
				Debug.Log($"equipIndex : {equipIndex}");
				weaponManager.AddWeapon(inventory.itemDatabase.itemObjects[newWeapon.itemId].weaponPrefab, equipIndex);

				//���� ���� ���Ⱑ ������� �κ��丮�� �߰�
				if(oldItem != null)
				{
					inventory.AddItem(oldItem, true);
				}
				isEquiped = true;
			}

			if (isEquiped == true)
			{
				//���â ���ΰ�ħ
				OnEquipChange?.Invoke(oldItem, newWeapon);
			}
			return isEquiped;
		}

		//�Ҹ� ���� ����
		public bool EquipConsumWeapon(Item newItem, int equipIndex = -1)
		{
			if(GetEquipIndex(newItem.itemId) != 1)
			{
				Debug.Log("�߸��� ������ ���� �õ�");
				return false;
			}

			bool isEquiped = false;

			Item oldItem = null;
			//�������� ���� ������ ���� �õ���
			if (equipIndex == -1)
			{
				//����ִ� ���� ���Կ� ������ ����, ����ִ� ������ ������� ����ó��
				for (int i = 0; i < consumWeaponItems.Length; i++)
				{
					if (consumWeaponItems[i].itemId == newItem.itemId && consumWeaponItems[i].amount < consumWeaponItems[i].maxStack)
					{
						consumWeaponItems[i].amount += newItem.creatAmount;
						isEquiped = true;
						break;
					}

					if (consumWeaponItems[i].itemId == -1)
					{
						consumWeaponItems[i] = newItem;
						weaponManager.AddWeapon(inventory.itemDatabase.itemObjects[newItem.itemId].weaponPrefab);
						isEquiped = true;
						break;
					}
					Debug.Log("�Ҹ� ����ĭ�� ��á���ϴ�.");
				}
			}
			//������ �����ϰ� ������ ���� �õ���
			else
			{
				//���� ������ ���Ⱑ ������� �� ���� ����
				oldItem = mainWeaponItems[equipIndex];
				//���Կ� ���ο� ���� ����
				mainWeaponItems[equipIndex] = newItem;

				weaponManager.AddWeapon(inventory.itemDatabase.itemObjects[newItem.itemId].weaponPrefab, equipIndex + consumedWeaponIndex);

				//���� ���� ���Ⱑ ������� �κ��丮�� �߰�
				if (oldItem != null)
				{
					inventory.AddItem(oldItem, oldItem.amount, true);
				}
				isEquiped = true;
			}

			if (isEquiped)
			{
				//���â ���ΰ�ħ
				OnEquipChange?.Invoke(oldItem, newItem);
			}
			return isEquiped;
		}

		//�� ����
		public void EquipItem(Item newItem)
		{
			bool isChanged = true;

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
				isChanged = false;
			}

			if (isChanged == true)
			{
				//UI ���ΰ�ħ
				OnEquipChange?.Invoke(oldItem, newItem);
			}
		}

		public int GetEquipIndex(int itemId)
		{
			ItemType itemType = inventory.itemDatabase.itemObjects[itemId].type;

			return (int)itemType;
		}

		public void UseConsumWeapon(int itemId, int useAmount)
		{
			for(int i = 0; i < consumWeaponItems.Length; i++)
			{
				if (consumWeaponItems[i].itemId == itemId)
				{
					consumWeaponItems[i].amount -= useAmount;
					if (consumWeaponItems[i].amount <= 0)
					{
						consumWeaponItems[i] = null;
                        consumWeaponItems[i] = new Item();
                    }
				}
			}
		}

		public int SetConsumWeaponAmount(int itemId)
		{
			int amount = 0;
            for (int i = 0; i < consumWeaponItems.Length; i++)
            {
                if (consumWeaponItems[i].itemId == itemId)
                {
					amount += consumWeaponItems[i].amount;
					return amount;
                    
                }
            }

			return amount;
        }

		//������ ���� ����
		public void UnequipItems(int slotIndex, EquipSlotType slotType)
		{
			Item changedItem = null;
			if (slotType == EquipSlotType.MainWeapon)
			{
				changedItem = mainWeaponItems[slotIndex];
				mainWeaponItems[slotIndex] = null;
				mainWeaponItems[slotIndex] = new Item();

				weaponManager.RemoveWeapon(slotIndex);
			}
			else if (slotType == EquipSlotType.ConsumWeapon)
			{
				changedItem = consumWeaponItems[slotIndex];
				consumWeaponItems[slotIndex] = null;
				consumWeaponItems[slotIndex] = new Item();

				weaponManager.RemoveWeapon(slotIndex + 3);
			}
			else
			{
				changedItem = equipItems[slotIndex];
				equipItems[slotIndex] = new Item();
			}

            Inventory.Instance.AddItem(changedItem);
            OnEquipChange?.Invoke(changedItem, null);
		}

		public void SwapWeaponSlot(int selectIndex, int targetIndex, EquipSlotType slotType)
		{
			Item oldItem;
			Item newItem;

			if(slotType == EquipSlotType.MainWeapon)
			{
				oldItem = mainWeaponItems[selectIndex];
				newItem = mainWeaponItems[targetIndex];

				mainWeaponItems[selectIndex] = mainWeaponItems[targetIndex];
				mainWeaponItems[targetIndex] = oldItem;

				weaponManager.SwapWeaponIndex(selectIndex, targetIndex);
			}
			else if(slotType == EquipSlotType.ConsumWeapon)
			{
                oldItem = consumWeaponItems[selectIndex];
                newItem = consumWeaponItems[targetIndex];

                consumWeaponItems[selectIndex] = consumWeaponItems[targetIndex];
				consumWeaponItems[targetIndex] = oldItem;

				weaponManager.SwapWeaponIndex(selectIndex + 3, targetIndex + 3);
			}
			else
			{
				return;
			}

			OnEquipChange?.Invoke(oldItem, newItem);
		}
	}
}