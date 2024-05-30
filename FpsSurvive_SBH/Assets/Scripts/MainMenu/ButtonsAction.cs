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


        #endregion

        public void PressStartButton()
        {
            
        }

        public void PressOptionButton()
        {

        }

        public void PressQuitButton()
        {
            Debug.Log("게임종료");
            Application.Quit();
        }
    }
}