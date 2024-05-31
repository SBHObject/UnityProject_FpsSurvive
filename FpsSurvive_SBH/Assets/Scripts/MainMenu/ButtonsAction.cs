using FpsSurvive.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FpsSurvive.Main
{
    public class ButtonsAction : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private string tutorialSceneName;

        private OptionUI optionUI;

        #endregion
        private void Start()
        {
            optionUI = GetComponent<OptionUI>();
        }


        public void PressStartButton()
        {
            
        }

        public void PressOptionButton()
        {
            optionUI.OpenOptionUI();
        }

        public void PressQuitButton()
        {
            Debug.Log("게임종료");
            Application.Quit();
        }
    }
}