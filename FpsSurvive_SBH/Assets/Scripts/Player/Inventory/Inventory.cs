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
		//게임 전체 아이템
		public ItemDatabase itemDatabase;

        //인벤토리속 아이템 목록
        public List<Item> items = new List<Item>();

		//기본 인벤토리 크기
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

		//인벤토리 빈칸찾기
		public bool IsEmpty(Item newItem)
		{
			bool empty = items.Count < FinalInvenSize;

			//스택 가능한 아이템일경우
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

		//인벤토리 아이템 추가
		public bool AddItem(Item newitem, bool exchange = false)
		{
			if((IsEmpty(newitem) == false && exchange == false) || itemDatabase.itemObjects[newitem.itemId].maxStack > unstackableItemStack)
			{
				Debug.Log("인벤토리 공간 부족 or 잘못된 요청");
				return false;
			}

			items.Add(newitem);

			OnItemChanged?.Invoke();
			return true;
		}

		//총알 획득시에 사용(한번에 다수의 스택 획득)
		public bool AddItem(Item newItem, int amount, bool exchange = false)
		{
			if((IsEmpty(newItem) == false && exchange == false) || itemDatabase.itemObjects[newItem.itemId].maxStack == unstackableItemStack)
			{
				Debug.Log("추가 실패, 공간이 부족하거나 잘못된 요청입니다(비 스택형 아이템 스택아이템 획득 요청)");
				return false;
			}

			//같은 아이템 보유 여부 체크
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
							//일단 나머지는 소멸, 이후 획득시 반환하도록 개선해볼것
						}
						break;
					}
				}
			}

			//같은 아이템이 없을경우 인벤토리에 추가
			if(isFind == false)
			{
				items.Add(newItem);
			}

			OnItemChanged?.Invoke();
			return true;
		}

		//비스택형 아이템 제거
		public void RemoveItem(Item oldItem)
		{
			if (itemDatabase.itemObjects[oldItem.itemId].maxStack > unstackableItemStack)
			{
				Debug.Log("잘못된 요청입니다(스택형 아이템인데 비스택형 아이템 제거 요청 시도)");
			}

			items.Remove(oldItem);

			OnItemChanged?.Invoke();
		}

		public void RemoveItem(Item oldItem, int amount)
		{
			if(itemDatabase.itemObjects[oldItem.itemId].maxStack == unstackableItemStack)
			{
				Debug.Log("잘못된 요청입니다(비스택형 아이템인데 스택형 아이템 제거 요청 시도)");
			}

			//삭제시 최대치 입력을 현재 보유량으로 제한할것...
			oldItem.amount -= amount;
			if(oldItem.amount <= 0)
			{
				items.Remove(oldItem);
			}

			OnItemChanged?.Invoke();
		}

		//아이템 위치 변경
		public void SwapItem(int indexA, int indexB)
		{
			if(indexA == indexB)
					return;

			Item temp = items[indexB];
			items[indexB] = items[indexA];
			items[indexA] = temp;

			OnItemChanged?.Invoke();
		}

		//장비아이템 체크
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