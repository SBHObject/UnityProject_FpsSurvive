using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FpsSurvive.AnimationParameter;
using static UnityEngine.Rendering.DebugUI;

namespace FpsSurvive.Player
{
    public class PlayerMove : MonoBehaviour
    {
        #region Variables
        private CharacterController _charCtrl;
        private PlayerInput _playerInput;
        private PlayerNewInput _input;

        //카메라 위치
        public GameObject camRoot;
        private float _camHorizontal;
        private float _camVertical;

        //위아래 카메라각 최소, 최대치 지정
        private float topClamp = 90;
        private float bottomClamp = -80;
        //카메라 민감도 설정
        private float camSensativity = 0.05f;
        
        //이동관련 변수
        public float walkSpeed = 5f;
        public float runSpeed = 10f;
        [SerializeField] private float rotationSpeed = 1f;
        private float speedCangeRate = 10f; // 플레이어 가속도
        //실제 플레이어가 움직일 속도
        private float _speed;
        
        //점프력
        private float jumpHeight = 1.2f;

        //바닥판정
        private float groundOffset = 0.1f;
        public bool IsGrounded { get; private set; }
        private float checkRange = 0.25f;
        public LayerMask layerMask;

        //낙하, 점프 시간 제힌
        private float fallTimeOut = 0.15f;
        private float jumpTimeOut = 0.1f;
        private float gravity = -15f;

        //낙하,점프 지속 시간
        private float _fallTimeDelta;
        private float _jumpTimeDelta;
        private float _verticalVelocity;

        //움직임 판정 최소치
        private const float _threshold = 0.01f;

        private float _termenalVelocity = 53.0f;

        //애니메이션 관련
        private Animator _ani;
        
        #endregion

        private void Start()
        {
            _charCtrl = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            _input = GetComponent<PlayerNewInput>();
            _ani = GetComponent<Animator>();
            

            _fallTimeDelta = fallTimeOut;
            _jumpTimeDelta = jumpTimeOut;
        }

        private void Update()
        {
            GroundedCheck();
            CameraRotation();
            Move();
            JumpAndGravity();
        }

        private void GroundedCheck()
        {
            Vector3 spherePos = new Vector3(transform.position.x, transform.position.y + groundOffset, transform.position.z);
            IsGrounded = Physics.CheckSphere(spherePos, checkRange, layerMask, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if(_input.look.magnitude >= _threshold)
            {
                _camVertical += _input.look.y * rotationSpeed * -1f * camSensativity;
                _camHorizontal = _input.look.x * rotationSpeed * camSensativity;

                _camVertical = ClampAngle(_camVertical, bottomClamp, topClamp);

                camRoot.transform.localRotation = Quaternion.Euler(_camVertical, 0, 0);
                transform.Rotate(Vector3.up * _camHorizontal);
            }
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? runSpeed : walkSpeed;

            if (_input.move == Vector2.zero) targetSpeed = 0;

            float currentSpeed = new Vector3(_charCtrl.velocity.x, 0, _charCtrl.velocity.z).magnitude;

            float speedOffset = 0.1f;

            if(currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentSpeed, targetSpeed, speedCangeRate * Time.deltaTime);

                _speed = Mathf.Round(_speed * 1000) / 1000;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDir = new Vector3(_input.move.x, 0, _input.move.y).normalized;

            if(_input.move != Vector2.zero)
            {
                inputDir = transform.right * _input.move.x + transform.forward * _input.move.y;
				_ani.SetBool(AniParameters.isMove, true);
			}
            else
            {
				_ani.SetBool(AniParameters.isMove, false);
			}

            //애니메이터에 파라미터 전달, 이동 애니메이션 구현
            float aniParaX = _speed * _input.move.x;
			float aniParaY = _speed * _input.move.y;
            _ani.SetFloat(AniParameters.xMoveSpeed, aniParaX);
			_ani.SetFloat(AniParameters.yMoveSpeed, aniParaY);

            //이동 구현
            _charCtrl.Move(inputDir.normalized * (_speed * Time.deltaTime) + new Vector3(0,_verticalVelocity,0) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if(IsGrounded)
            {
                //땅에 있을때
                _fallTimeDelta = fallTimeOut;   //지상유지중

                //기본상태, 받는 중력을 -2로 변경
                if (_verticalVelocity < 0)
                {
                    _verticalVelocity = -2f;
                }

                //점프입력시, _jumpTimeDelta가 0이하여야 점프가 가능함
                if (_input.jump && _jumpTimeDelta <= 0)
                {
                    //y 축으로 move 이동, Move()의 _charCtrl.Move 에서 연산
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
                }

                //지상에 있을때, _jumpTimeDelta를 0 이하로 유지함
                if(_jumpTimeDelta >= 0)
                {
                    _jumpTimeDelta -= Time.deltaTime;
                }
            }
            else
            {
                //땅에서 떨어졌을때
                _jumpTimeDelta = jumpTimeOut;   //점프 불가능, 땅에 닿기 전까지 timeDelta를 0.1로 초기화함

                //낙하판정 여부
                if (_fallTimeDelta >= 0)
                {
                    //땅에서 발이 떨어지면 _fallTimeDelta 감소, 0.15보다 낮아지면 추락중 판정(계단, 점프)
                    _fallTimeDelta -= Time.deltaTime;
                }

                //땅에서 떨어져있을때, 점프 입력을 false로 유지
                _input.jump = false;
            }

            //점프 시도중이 아닐경우, 중력을 지속적으로 적용, 낙하상태가 유지되면 낙하속도가 가속됨
            if(_verticalVelocity < _termenalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (IsGrounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + groundOffset, transform.position.z), checkRange);
        }
    }
}