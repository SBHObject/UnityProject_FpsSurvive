using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FpsSurvive.Player;
using static UnityEditor.Timeline.Actions.MenuPriority;
using UnityEditorInternal.Profiling.Memory.Experimental;

namespace FpsSurvive.Game
{
    public class StatsCheckUI : MonoBehaviour
    {
        public StatsObject playerStats;

        public TextMeshProUGUI dmgReduce;
        public TextMeshProUGUI dmgIncrese;
        public TextMeshProUGUI critRate;
        public TextMeshProUGUI moveSpeed;
        public TextMeshProUGUI backpackSize;
        public TextMeshProUGUI goldGain;

        private void Start()
        {
            Equipment.Instance.OnEquipChange += OnEquipChanged;
            playerStats.OnChangeStats += OnChangeStats;

            UpdateText();
        }

        private void UpdateText()
        {
            dmgReduce.text = playerStats.GetModifiredValue(AttributeType.DamageReduce).ToString();
            dmgIncrese.text = playerStats.GetModifiredValue(AttributeType.DamageIncrese).ToString();
            critRate.text = playerStats.GetModifiredValue(AttributeType.CriticalRate).ToString();
            moveSpeed.text = playerStats.GetModifiredValue(AttributeType.MoveSpeed).ToString();
            backpackSize.text = playerStats.GetModifiredValue(AttributeType.BackpackSize).ToString();
            goldGain.text = playerStats.GetModifiredValue(AttributeType.GoldGainIncrease).ToString();
        }

        private void OnChangeStats(StatsObject statsObject)
        {
            UpdateText();
        }

        private void OnEquipChanged(Item oldItem, Item newItem)
        {
            if (oldItem != null && oldItem.itemId > -1)
            {
                foreach (var buff in oldItem.buffs)
                {
                    foreach (var attribute in playerStats.attributes)
                    {
                        if (attribute.type == buff.stat)
                        {
                            attribute.value.RemoveModifier(buff._value);
                        }
                    }
                }
            }

            if (newItem != null && newItem.itemId > -1)
            {
                foreach (var buff in newItem.buffs)
                {
                    foreach (var attribute in playerStats.attributes)
                    {
                        if (attribute.type == buff.stat)
                        {
                            attribute.value.AddModifier(buff._value);
                        }
                    }
                }
            }
        }
    }
}