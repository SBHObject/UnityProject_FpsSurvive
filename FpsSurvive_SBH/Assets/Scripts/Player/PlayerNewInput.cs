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
    }
}