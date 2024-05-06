using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FpsSurvive.Player;

namespace FpsSurvive.Game
{
    public class Interactive : MonoBehaviour
    {
        #region Variables
        protected float distance;

        [SerializeField]
        private bool unInteractive = false;

        [SerializeField]
        protected string actionText = "Press 'E' To Pickup";

        private PlayerNewInput thePlayer;
        #endregion

        private void OnEnable()
        {
            thePlayer = FindObjectOfType<PlayerNewInput>();
        }

        private void Update()
        {
            distance = PlayerCasting.distanceToTarget;
        }

        public virtual void DoAction()
        {

        }

        public string SetActionText()
        {
            return actionText;
        }
    }
}