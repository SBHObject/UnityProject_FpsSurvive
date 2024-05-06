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

		protected bool isOpen = false;

		protected PlayerMove playerMove;
        protected PlayerNewInput playerNewInput;
        protected PlayerWeaponManager weaponManager;

		[SerializeField]
		protected float animatingTimeLenght = 0.3f;
		#endregion

		protected virtual void Awake()
		{
			ani = ThisUI.GetComponent<Animator>();
			playerMove = FindObjectOfType<PlayerMove>();
			playerNewInput = FindObjectOfType<PlayerNewInput>();
			weaponManager = FindObjectOfType<PlayerWeaponManager>();
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

		protected virtual void UIOpen()
		{
			StartCoroutine(OpenUIAni());
		}

		protected void UIClose()
		{
			StartCoroutine(CloseUIAni());
		}

		protected virtual IEnumerator OpenUIAni()
		{
			isOpen = true;
			playerMove.enabled = false;
			playerNewInput.enabled = false;
			weaponManager.enabled = false;
			ani.SetBool(AniParameters.isOpen, true);

			yield return new WaitForSeconds(animatingTimeLenght);

			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		protected virtual IEnumerator CloseUIAni()
		{
			isOpen = false;
			ani.SetBool(AniParameters.isOpen, false);

			yield return new WaitForSeconds(animatingTimeLenght);

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			playerMove.enabled = true;
			playerNewInput.enabled = true;
            weaponManager.enabled = true;
        }
	}
}