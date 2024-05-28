using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FpsSurvive.Utility;

namespace FpsSurvive.Player
{
    public class PlayerStats : PersistantSingleton<PlayerStats>
    {
		#region Variables
		//������ ���� ����
		public StatsObject playerStats;

		public int DamageReduce {  get; private set; }
		public int DamageIncrese {  get; private set; }
		public int CriticalRate { get; private set; }
		public int MoveSpeed { get; private set; }
		public int BackpackSize {  get; private set; }
		public int GoldGainIncrese { get; private set; }

		private int gold;

		public int Gold 
		{
			get { return gold; }
			private set {  gold = value; } 
		}

		public bool IsDeath { get; set; }
        #endregion

        private void Start()
        {
			Equipment.Instance.OnEquipChange += OnEquipChanged;
			playerStats.OnChangeStats += OnStatChanged;

			//���� �ʱⰪ ����
			UpdateStats();
        }

		private void OnStatChanged(StatsObject statsObject)
		{
			UpdateStats();
		}

		//���� ���� ���� ����
		private void UpdateStats()
		{
			DamageReduce = playerStats.GetModifiredValue(AttributeType.DamageReduce);
			DamageIncrese = playerStats.GetModifiredValue(AttributeType.DamageIncrese);
			CriticalRate = playerStats.GetModifiredValue(AttributeType.CriticalRate);
			MoveSpeed = playerStats.GetModifiredValue(AttributeType.MoveSpeed);
			BackpackSize = playerStats.GetModifiredValue(AttributeType.BackpackSize);
			GoldGainIncrese = playerStats.GetModifiredValue(AttributeType.GoldGainIncrease);
		}

		//������ �����, ���� ����
        private void OnEquipChanged(Item oldItem, Item newItem)
		{
			if(oldItem != null && oldItem.itemId > -1)
			{
				foreach(var buff in oldItem.buffs)
				{
					foreach(var attribute in playerStats.attributes)
					{
						if(attribute.type == buff.stat)
						{
							attribute.value.RemoveModifier(buff._value);
						}
					}
				}
			}

			if(newItem != null && newItem.itemId > -1)
			{
				foreach(var buff in newItem.buffs)
				{
					foreach(var attribute in playerStats.attributes)
					{
						if(attribute.type == buff.stat)
						{
							attribute.value.AddModifier(buff._value);
						}
					}	
				}
			}
		}

		public void AddGold(int amount)
		{
			Gold += amount;
		}

		public bool UseGold(int amount)
		{
			if(Gold < amount)
			{
				return false;
			}

			Gold -= amount;
			return true;
		}
	}
}