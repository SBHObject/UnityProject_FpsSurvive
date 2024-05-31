using FpsSurvive.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FpsSurvive.Main
{
    public class OptionManager : PersistantSingleton<OptionManager>
    {
        #region Variables
        private float sfxVolum;
        public float SfxVolum
        {
            get { return sfxVolum; }
            set { sfxVolum = value; }
        }

        private float mouseSensitivity;
        public float MouseSensitivity
        {
            get { return mouseSensitivity; }
            set { mouseSensitivity = value; }
        }

        public AudioMixer audioMixer;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            SfxVolum = PlayerPrefs.GetFloat(PlayerPrefsKeyName.volumKey, 1f);
            MouseSensitivity = PlayerPrefs.GetFloat(PlayerPrefsKeyName.mouseSensitiveKey, 1.0f);
        }


        public void SetSfxVolum(float amount)
        {
            SfxVolum = amount;
            PlayerPrefs.SetFloat(PlayerPrefsKeyName.volumKey, amount);
        }

        public void SetMouseSensitive(float  amount)
        {
            MouseSensitivity = amount;
            PlayerPrefs.SetFloat(PlayerPrefsKeyName.mouseSensitiveKey, amount);
        }
    }
}