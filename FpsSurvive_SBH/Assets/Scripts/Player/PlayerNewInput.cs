using FpsSurvive.AnimationParameter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FpsSurvive.Player
{
    public class PlayerNewInput : MonoBehaviour
    {
        public Vector2 move;
        public Vector2 look;

        public bool jump;
        public bool sprint;
        public bool shoot;
        public bool m_ShootIsHeld;

        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        private Animator _ani;

		private void LateUpdate()
		{
            m_ShootIsHeld = OnShootHold();
		}

		private void Start()
		{
			_ani = GetComponent<Animator>();
		}

		public void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            look = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            jump = value.isPressed;
		}

        public void OnSprint(InputValue value)
        {
            sprint = value.isPressed;
        }

        public void OnShoot(InputValue value)
        {
            shoot = value.isPressed;
		}

        public bool OnShootHold()
        {
            return shoot;
        }

        public bool OnShootDown()
        {
            return OnShootHold() && !m_ShootIsHeld;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public int GetSelectWeaponInput()
        {
			if (Input.GetKeyDown(KeyCode.Alpha1))
				return 1;
			else if (Input.GetKeyDown(KeyCode.Alpha2))
				return 2;
			else if (Input.GetKeyDown(KeyCode.Alpha3))
				return 3;
			else if (Input.GetKeyDown(KeyCode.Alpha4))
				return 4;
			else if (Input.GetKeyDown(KeyCode.Alpha5))
				return 5;
			else if (Input.GetKeyDown(KeyCode.Alpha6))
				return 6;
			else if (Input.GetKeyDown(KeyCode.Alpha7))
				return 7;
			else if (Input.GetKeyDown(KeyCode.Alpha8))
				return 8;
			else if (Input.GetKeyDown(KeyCode.Alpha9))
				return 9;
			else
				return 0;
		}
    }
}