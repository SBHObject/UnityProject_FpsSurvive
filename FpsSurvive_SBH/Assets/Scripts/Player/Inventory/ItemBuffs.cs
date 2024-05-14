using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    public enum AttributeType
    {
        DamageReduce,
        DamageIncrese,
        CriticalRate,
        MoveSpeed,
        BackpackSize,
        GoldGainIncrease
    }

    [System.Serializable]
    public class ItemBuffs : IModifier
    {
        public AttributeType stat;
        public int _value;

        [SerializeField]
        private int minValue;
        [SerializeField]
        private int maxValue;

        public int Min => minValue;
        public int Max => maxValue;

        public ItemBuffs(int min, int max)
        {
            minValue = min;
            maxValue = max;
            GenerateValue();
        }

        private void GenerateValue()
        {
            _value = Random.Range(minValue, maxValue);
        }

        public void AddValue(ref int value)
        {
            value += _value;
        }
    }
}