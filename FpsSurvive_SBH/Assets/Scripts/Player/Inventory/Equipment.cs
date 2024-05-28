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

		//주무기류 장착
		public bool EquipMainWeapon(Item newWeapon, int equipIndex = -1)
		{
			//0번 타입이 아닌 아이템 장착 시도시 실패처리
			if (GetEquipIndex(newWeapon.itemId) != 0)
			{
				Debug.Log("잘못된 아이템 장착 시도");
				return false;
			}

			bool isEquiped = false;

			Item oldItem = null;

			//슬롯지정 없이 아이템 장착 시도시
			if(equipIndex == -1)
			{
				//비어있는 무기 슬롯에 아이템 장착, 비어있는 슬롯이 없을경우 실패처리
				for(int i = 0; i <mainWeaponItems.Length; i++)
				{
					if(mainWeaponItems[i].itemId == -1)
					{
						Debug.Log("빈칸");
						mainWeaponItems[i] = newWeapon;
						weaponManager.AddWeapon(inventory.itemDatabase.itemObjects[newWeapon.itemId].weaponPrefab);
						isEquiped = true;
						break;
					}
					Debug.Log("주무기칸이 꽉찼습니다.");
				}
			}
			//슬롯을 지정하고 아이템 장착 시도시
			else
			{
				//기존 장착된 무기가 있을경우 그 무기 저장
				oldItem = mainWeaponItems[equipIndex];
				//슬롯에 새로운 무기 저장
				mainWeaponItems[equipIndex] = newWeapon;
				Debug.Log($"equipIndex : {equipIndex}");
				weaponManager.AddWeapon(inventory.itemDatabase.itemObjects[newWeapon.itemId].weaponPrefab, equipIndex);

				//기존 장착 무기가 있을경우 인벤토리에 추가
				if(oldItem != null)
				{
					inventory.AddItem(oldItem, true);
				}
				isEquiped = true;
			}

			if (isEquiped == true)
			{
				//장비창 새로고침
				OnEquipChange?.Invoke(oldItem, newWeapon);
			}
			return isEquiped;
		}

		//소모성 무기 장착
		public bool EquipConsumWeapon(Item newItem, int equipIndex = -1)
		{
			if(GetEquipIndex(newItem.itemId) != 1)
			{
				Debug.Log("잘못된 아이템 장착 시도");
				return false;
			}

			bool isEquiped = false;

			Item oldItem = null;
			//슬롯지정 없이 아이템 장착 시도시
			if (equipIndex == -1)
			{
				//비어있는 무기 슬롯에 아이템 장착, 비어있는 슬롯이 없을경우 실패처리
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
					Debug.Log("소모성 무기칸이 꽉찼습니다.");
				}
			}
			//슬롯을 지정하고 아이템 장착 시도시
			else
			{
				//기존 장착된 무기가 있을경우 그 무기 저장
				oldItem = mainWeaponItems[equipIndex];
				//슬롯에 새로운 무기 저장
				mainWeaponItems[equipIndex] = newItem;

				weaponManager.AddWeapon(inventory.itemDatabase.itemObjects[newItem.itemId].weaponPrefab, equipIndex + consumedWeaponIndex);

				//기존 장착 무기가 있을경우 인벤토리에 추가
				if (oldItem != null)
				{
					inventory.AddItem(oldItem, oldItem.amount, true);
				}
				isEquiped = true;
			}

			if (isEquiped)
			{
				//장비창 새로고침
				OnEquipChange?.Invoke(oldItem, newItem);
			}
			return isEquiped;
		}

		//방어구 장착
		public void EquipItem(Item newItem)
		{
			bool isChanged = true;

			//장착할 인덱스 지정 (방어구 계열 타입 2 ~ 5, 각각 0 ~ 3까지 장착)
			int index = GetEquipIndex(newItem.itemId) - armorTypeIndex;
			//장착중인 아이템을 저장(없을경우 null 저장)
			Item oldItem = equipItems[index];
			//장착아이템에 장착 시도한 아이템 적용
			equipItems[index] = newItem;
			//장착중인 아이템이 있었을경우, 해당 아이템 인벤토리에 추가
			if(oldItem != null && oldItem.itemId >= armorTypeIndex)
			{
				inventory.AddItem(oldItem , true);
				isChanged = false;
			}

			if (isChanged == true)
			{
				//UI 새로고침
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

		//아이템 장착 해제
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