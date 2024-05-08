using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.UI
{
    public class PauseUI : MonoBehaviour
    {
		#region Variables
		private PlayerNewInput m_Input;

		public GameObject pauseUIObject;
		private bool isOpenUI = false;
		#endregion

		private void Start()
		{
			m_Input = FindObjectOfType<PlayerNewInput>();
		}

		private void Update()
		{
			if(m_Input.GetEscapeButton())
			{
				ToggleUI();
			}
		}

		private void ToggleUI()
		{
			if(isOpenUI == true)
			{
				CloseUI();
			}
			else
			{
				OpenUI();
			}
		}

		public void OpenUI()
		{
			isOpenUI = true;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			pauseUIObject.SetActive(true);
		}

		public void CloseUI()
		{
			isOpenUI = false;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			pauseUIObject.SetActive(false);
		}

		public void QuitButton()
		{
			Application.Quit();
		}
	}
}