using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FpsSurvive.Player;
using FpsSurvive.UI;
using FpsSurvive.AnimationParameter;
using UnityEngine.UI;

namespace FpsSurvive.Game
{
    public class StatsCheckUI : ItemUI
    {
        #region Variables
        public PlayerStats stats;

        public TextMeshProUGUI dmgReduce;
        public TextMeshProUGUI dmgIncrese;
        public TextMeshProUGUI critRate;
        public TextMeshProUGUI moveSpeed;
        public TextMeshProUGUI backpackSize;
        public TextMeshProUGUI goldGain;

        Inventory inventory;
        Equipment equipment;

        #endregion
        private void Start()
        {
            inventory = Inventory.Instance;
            equipment = Equipment.Instance;
            stats = PlayerStats.Instance;

            equipment.OnEquipChange += OnChangeStats;

            UpdateText();
        }

        protected override IEnumerator OpenUIAni()
        {
            isOpen = true;

            ThisUI.SetActive(true);
            ani.SetBool(AniParameters.isOpen, true);

            yield return new WaitForSeconds(animatingTimeLenght);
        }

        protected override IEnumerator CloseUIAni()
        {
            isOpen = false;
            ani.SetBool(AniParameters.isOpen, false);

            yield return new WaitForSeconds(animatingTimeLenght);

            ThisUI.SetActive(false);
        }

        public void InfoOpen()
        {
            UIOpen();
        }

        public void InfoClose()
        {
            UIClose();
        }

        private void UpdateText()
        {
            dmgReduce.text = stats.playerStats.GetModifiredValue(AttributeType.DamageReduce).ToString();
            dmgIncrese.text = stats.playerStats.GetModifiredValue(AttributeType.DamageIncrese).ToString();
            critRate.text = stats.playerStats.GetModifiredValue(AttributeType.CriticalRate).ToString();
            moveSpeed.text = stats.playerStats.GetModifiredValue(AttributeType.MoveSpeed).ToString();
            backpackSize.text = stats.playerStats.GetModifiredValue(AttributeType.BackpackSize).ToString();
            goldGain.text = stats.playerStats.GetModifiredValue(AttributeType.GoldGainIncrease).ToString();
        }

        private void OnChangeStats(Item oldItem, Item newItem)
        {
            UpdateText();
        }
    }
}