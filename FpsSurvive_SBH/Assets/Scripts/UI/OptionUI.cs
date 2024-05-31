using FpsSurvive.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FpsSurvive.UI
{
    public class OptionUI : MonoBehaviour
    {
        #region Variables
        private OptionManager optionManager;

        public GameObject optionUI;

        public Slider sfxSlider;
        public Slider mouseSlider;

        public TextMeshProUGUI sfxValueText;
        public TextMeshProUGUI mouseValueText;

        private float sfxValue;
        private float mouseValue;
        #endregion
        private void Start()
        {
            optionManager = OptionManager.Instance;
            optionUI.SetActive(false);
        }


        public void OpenOptionUI()
        {
            optionUI.SetActive(true);

            sfxValue = optionManager.SfxVolum;
            sfxSlider.value = sfxValue;

            mouseValue = optionManager.MouseSensitivity;
            mouseSlider.value = mouseValue;


            sfxValueText.text = (Mathf.Floor(sfxValue * 10f) / 10f).ToString();
            mouseValueText.text = (Mathf.Floor(mouseValue * 10f) / 10f).ToString();
        }

        public void PressSavePutton()
        {
            optionManager.SetSfxVolum(sfxSlider.value);
            optionManager.SetMouseSensitive(mouseSlider.value);
        }
        
        public void PressCancelButton()
        {
            sfxSlider.value = optionManager.SfxVolum;
            mouseSlider.value = optionManager.MouseSensitivity;
        }

        public void PressCloseButton()
        {
            optionUI.SetActive(false);
        }

        public void SfxSliderMove()
        {
            sfxValueText.text = (Mathf.Floor(sfxSlider.value * 10f) / 10f).ToString();
        }

        public void MouseSliderMove()
        {
            mouseValueText.text = (Mathf.Floor(mouseSlider.value * 10f) / 10f).ToString();
        }

    }
}