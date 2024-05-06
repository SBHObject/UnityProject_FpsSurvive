using FpsSurvive.AnimationParameter;
using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace FpsSurvive.UI
{
    public class ItemInfoUI : ItemUI
    {
        #region Variables
        Inventory inventory;
        Equipment equipment;
        #endregion

        private void Start()
        {
            inventory = Inventory.Instance;
            equipment = Equipment.Instance;
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
    }
}