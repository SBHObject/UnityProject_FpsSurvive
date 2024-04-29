using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FpsSurvive.AnimationParameter;

namespace FpsSurvive.UI
{
    public class ItemUI : MonoBehaviour
    {
		#region Variables
		public GameObject ThisUI;
		protected Animator ani;

		private bool isOpen = false;

		private PlayerMove playerMove;
		private PlayerNewInput playerNewInput;

		[SerializeField]
		private float animatingTimeLenght = 0.3f;
		#endregion

		protected virtual void Awake()
		{
			ani = ThisUI.GetComponent<Animator>();
			playerMove = FindObjectOfType<PlayerMove>();
			playerNewInput = FindObjectOfType<PlayerNewInput>();
		}

		public void Toggle()
		{
			if(isOpen)
			{
				UIClose();
			}
			else
			{
				UIOpen();
			}
		}

		protected void UIOpen()
		{
			StartCoroutine(OpenUIAni());
		}

		protected void UIClose()
		{
			StartCoroutine(CloseUIAni());
		}

		protected IEnumerator OpenUIAni()
		{
			isOpen = true;
			playerMove.enabled = false;
			playerNewInput.enabled = false;
			ani.SetBool(AniParameters.isOpen, true);

			yield return new WaitForSeconds(animatingTimeLenght);

			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		protected IEnumerator CloseUIAni()
		{
			isOpen = false;
			ani.SetBool(AniParameters.isOpen, false);

			yield return new WaitForSeconds(animatingTimeLenght);

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			playerMove.enabled = true;
			playerNewInput.enabled = true;
		}
	}
}