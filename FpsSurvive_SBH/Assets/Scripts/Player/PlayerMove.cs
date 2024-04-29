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

        //ī�޶� ��ġ
        public GameObject camRoot;
        private float _camHorizontal;
        private float _camVertical;

        //���Ʒ� ī�޶� �ּ�, �ִ�ġ ����
        private float topClamp = 90;
        private float bottomClamp = -80;
        //ī�޶� �ΰ��� ����
        private float camSensativity = 0.05f;
        
        //�̵����� ����
        public float walkSpeed = 5f;
        public float runSpeed = 10f;
        [SerializeField] private float rotationSpeed = 1f;
        private float speedCangeRate = 10f; // �÷��̾� ���ӵ�
        //���� �÷��̾ ������ �ӵ�
        private float _speed;
        
        //������
        private float jumpHeight = 1.2f;

        //�ٴ�����
        private float groundOffset = 0.1f;
        public bool IsGrounded { get; private set; }
        private float checkRange = 0.25f;
        public LayerMask layerMask;

        //����, ���� �ð� ����
        private float fallTimeOut = 0.15f;
        private float jumpTimeOut = 0.1f;
        private float gravity = -15f;

        //����,���� ���� �ð�
        private float _fallTimeDelta;
        private float _jumpTimeDelta;
        private float _verticalVelocity;

        //������ ���� �ּ�ġ
        private const float _threshold = 0.01f;

        private float _termenalVelocity = 53.0f;

        //�ִϸ��̼� ����
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

            //�ִϸ����Ϳ� �Ķ���� ����, �̵� �ִϸ��̼� ����
            float aniParaX = _speed * _input.move.x;
			float aniParaY = _speed * _input.move.y;
            _ani.SetFloat(AniParameters.xMoveSpeed, aniParaX);
			_ani.SetFloat(AniParameters.yMoveSpeed, aniParaY);

            //�̵� ����
            _charCtrl.Move(inputDir.normalized * (_speed * Time.deltaTime) + new Vector3(0,_verticalVelocity,0) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if(IsGrounded)
            {
                //���� ������
                _fallTimeDelta = fallTimeOut;   //����������

                //�⺻����, �޴� �߷��� -2�� ����
                if (_verticalVelocity < 0)
                {
                    _verticalVelocity = -2f;
                }

                //�����Է½�, _jumpTimeDelta�� 0���Ͽ��� ������ ������
                if (_input.jump && _jumpTimeDelta <= 0)
                {
                    //y ������ move �̵�, Move()�� _charCtrl.Move ���� ����
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
                }

                //���� ������, _jumpTimeDelta�� 0 ���Ϸ� ������
                if(_jumpTimeDelta >= 0)
                {
                    _jumpTimeDelta -= Time.deltaTime;
                }
            }
            else
            {
                //������ ����������
                _jumpTimeDelta = jumpTimeOut;   //���� �Ұ���, ���� ��� ������ timeDelta�� 0.1�� �ʱ�ȭ��

                //�������� ����
                if (_fallTimeDelta >= 0)
                {
                    //������ ���� �������� _fallTimeDelta ����, 0.15���� �������� �߶��� ����(���, ����)
                    _fallTimeDelta -= Time.deltaTime;
                }

                //������ ������������, ���� �Է��� false�� ����
                _input.jump = false;
            }

            //���� �õ����� �ƴҰ��, �߷��� ���������� ����, ���ϻ��°� �����Ǹ� ���ϼӵ��� ���ӵ�
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