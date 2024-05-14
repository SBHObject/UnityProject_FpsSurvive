using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    [CreateAssetMenu(fileName = "New Stats", menuName = "Stats/new Character Stats")]
    public class StatsObject : ScriptableObject
    {
        #region Variables
        public Attribute[] attributes;

        public Action<StatsObject> OnChangeStats;

        private bool isInitialized = false;
        #endregion

        private void OnEnable()
        {
            InitializeAttributes();
        }

        public int GetBaseValue(AttributeType type)
        {
            foreach (var attribute in attributes)
            {
                if(attribute.type == type)
                {
                    return attribute.value.BaseValue;
                }
            }
            return -1;
        }

        public int GetModifiredValue(AttributeType type)
        {
            foreach(var attribute in attributes)
            {
                if(attribute.type == type)
                {
                    return attribute.value.ModifiedValue;
                }
            }
            return -1;
        }

        //加己蔼 眠啊
        public void AddModifiredValue(AttributeType type, int amount)
        {
            foreach( var attribute in attributes)
            {
                if(attribute.type == type)
                {
                    attribute.value.AddModifier(amount);
                }
            }
        }

        //扁夯蔼 积己
        private void SetBaseValue(AttributeType type, int value)
        {
            foreach(var attribute in attributes)
            {
                if(attribute.type == type)
                {
                    attribute.value.BaseValue = value;
                }
            }
        }

        private void OnModifiredValue(ModifiableInt value)
        {
            OnChangeStats?.Invoke(this);
        }

        private void InitializeAttributes()
        {
            isInitialized = true;

            //加己蔼 积己
            foreach(var attribute in attributes)
            {
                attribute.value = new ModifiableInt(OnModifiredValue);
            }

            SetBaseValue(AttributeType.DamageReduce, 0);
            SetBaseValue(AttributeType.DamageIncrese, 0);
            SetBaseValue(AttributeType.CriticalRate, 0);
            SetBaseValue(AttributeType.MoveSpeed, 0);
            SetBaseValue(AttributeType.BackpackSize, 0);
            SetBaseValue(AttributeType.GoldGainIncrease, 0);
        }
    }
}