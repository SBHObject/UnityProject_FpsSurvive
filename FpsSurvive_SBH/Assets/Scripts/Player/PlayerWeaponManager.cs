using FpsSurvive.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FpsSurvive.Player
{
	//���ⱳü ����
	public enum WeaponSwitchState
	{
		Up,                 //�⺻, ���⸦ ����ִ� ����
		Down,               //���⸦ ������ ����(���� ����� ��ü�ϱ� ����)
		PutDownPrevious,    //���⸦ ������ ����
		PutUpNew            //���⸦ �ø��� ����
	}

	public class PlayerWeaponManager : MonoBehaviour
    {
		#region Variables
		private PlayerNewInput m_Input;
		private PlayerMove m_PlayerMove;

		public Camera weaponCam;

		//�������� ó�� �����ϴ� ���� ����Ʈ - ������ ����Ʈ
		public List<WeaponController> startingWeapons = new List<WeaponController>();

		//���Ⱑ �����Ǵ� �θ� ������Ʈ
		public Transform weaponParentSocket;

		//�÷��̾ �����߿� ���ٴϴ� ���� �迭
		[SerializeField]
		private WeaponController[] weaponSlots = new WeaponController[5];
		//���� ���� �迭�� �����ϴ� �ε���
		public int ActiveWeaponIndex { get; private set; }

		private const int mainWeaponIndex = 3;
		private const int consumedWeaponIndex = 5;

		private WeaponSwitchState weaponSwitchState;        //���� ��ü ����
		private Vector3 weaponMainLocalPosition;        //���� ��ġ��(�⺻)

		public Transform defaultWeaponPosition;
		public Transform downWeaponPosition;

		private int weaponSwitchNewWeaponIndex;           //���� ��ü�� ���� �ε���
		[SerializeField]
		private float weaponSwitchDelay = 1f;           //
		private float weaponSwitchTimeStared = 0f;      //���� ��ü Ÿ�̸�(��ü��, �ٸ��ൿ ����)

		private Vector3 m_lastCharPosition;

		//���� ��ü�� ȣ��Ǵ� �̺�Ʈ �Լ�
		public UnityAction<WeaponController> OnSwitchToWeapon;
		#endregion

		private void Awake()
		{
			m_Input = GetComponent<PlayerNewInput>();
			m_PlayerMove = GetComponent<PlayerMove>();
		}

		private void Start()
		{
			//�ʱ�ȭ
			ActiveWeaponIndex = -1;
			weaponSwitchState = WeaponSwitchState.Down;
			weaponSwitchTimeStared = Time.time;

			//�̺�Ʈ�Լ� ���
			OnSwitchToWeapon += OnWeaponSwitch;

			//���۹��� ����
			foreach (var weapon in startingWeapons)
			{
				AddWeapon(weapon);
			}

			SwitchWeapon(true);
		}

		private void Update()
		{
			WeaponController weaponContoller = GetActiveWeapon();

			//���� ��ü ��ǲ
			if (weaponSwitchState == WeaponSwitchState.Up || weaponSwitchState == WeaponSwitchState.Down)
			{
				int switchInput = m_Input.GetSelectWeaponInput(); //���� ����
				if (switchInput != 0)
				{
					if(GetWeaponAtSlotIndex(switchInput - 1) != null)
					{
						SwitchToWeaponIndex(switchInput - 1);
					}
				}
			}

			if (weaponSwitchState == WeaponSwitchState.Up)
			{
				bool hasFired = weaponContoller.HandleShootInput(m_Input.OnShootDown(), m_Input.OnShootHold());
			}
		}

		private void LateUpdate()
		{
			UpdateWeaponSwitching();

			//����� ������ ���� ��ġ�� transform�� ����
			weaponParentSocket.localPosition = weaponMainLocalPosition;
		}

		//�̵��� ���� ���� ��鸲
		private void WeaponBobing()
		{
			if (Time.deltaTime > 0)
			{
				Vector3 playerVelocity = (m_PlayerMove.transform.position - m_lastCharPosition) / Time.deltaTime;


				float charMoveFactor = 0f;
				if (m_PlayerMove.IsGrounded)
				{
					charMoveFactor = Mathf.Clamp01(playerVelocity.magnitude / m_PlayerMove.walkSpeed);
				}
				m_lastCharPosition = m_PlayerMove.transform.position;
			}
		}

		//���� ���¿� ���� ���ⱳü ����, ���� ��ġ ���� (weaponMainLoalPosition)
		private void UpdateWeaponSwitching()
		{
			//Lerp ���� : 0 ~ 1 ���� �� ��, A -> B �� ��ȯ
			float switchingTimeFactor = 0f;
			if (weaponSwitchDelay == 0f)
			{
				//1�� ��� B ���� ����
				switchingTimeFactor = 1f;
			}
			else
			{
				//���� �ð� - ���� �ð� = ������ �ð�, Clamp�� ���� 1�� ������ 1�� ����
				switchingTimeFactor = Mathf.Clamp01((Time.time - weaponSwitchTimeStared) / weaponSwitchDelay);
			}

			//�̵� ���� Ÿ�̸� �Ϸ� �� ����ó��
			if (switchingTimeFactor >= 1f)
			{
				if (weaponSwitchState == WeaponSwitchState.PutDownPrevious)
				{
					//���� ����ִ¹��� false, ���ο�� true
					WeaponController oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
					if (oldWeapon != null)
					{
						oldWeapon.ShowWeapon(false);
					}
					//�������� false �Ϸ�
					//���ο� ���⸦ Active ���·� ����
					ActiveWeaponIndex = weaponSwitchNewWeaponIndex;
					WeaponController newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
					OnSwitchToWeapon?.Invoke(newWeapon);

					//��ü���� �غ�
					switchingTimeFactor = 0f;
					if (newWeapon != null)
					{
						weaponSwitchTimeStared = Time.time;
						weaponSwitchState = WeaponSwitchState.PutUpNew;
					}
					else
					{
						weaponSwitchState = WeaponSwitchState.Down;
					}
				}
				else if (weaponSwitchState == WeaponSwitchState.PutUpNew)
				{
					weaponSwitchState = WeaponSwitchState.Up;
				}
			}

			//���� �̵� ����
			if (weaponSwitchState == WeaponSwitchState.PutDownPrevious) //�� -> �Ʒ�
			{
				//������ �Ʒ��� �̵�
				weaponMainLocalPosition = Vector3.Lerp(defaultWeaponPosition.localPosition, downWeaponPosition.localPosition, switchingTimeFactor);
			}
			else if (weaponSwitchState == WeaponSwitchState.PutUpNew)   //�Ʒ� -> ��
			{
				//�Ʒ����� ���� �̵�
				weaponMainLocalPosition = Vector3.Lerp(downWeaponPosition.localPosition, defaultWeaponPosition.localPosition, switchingTimeFactor);
			}
		}

		//���� �߰�
		public bool AddWeapon(WeaponController newWeapon)
		{
			if (newWeapon == null || HasWeapon(newWeapon) != null)
			{
				Debug.Log("���� ���⸦ �����ϰ��ְų�, �߸��� ��û�Դϴ�");
				return false;
			}

			if (newWeapon.slotType == WeaponSlotType.Main)
			{
				for (int i = 0; i < mainWeaponIndex	; i++)
				{
					//����ִ� ����üũ�ؼ� �� ���Կ� ���� �߰�
					if (weaponSlots[i] == null)
					{
						WeaponController weaponInstace = Instantiate(newWeapon, weaponParentSocket);
						weaponInstace.transform.localPosition = Vector3.zero;
						weaponInstace.transform.localRotation = Quaternion.identity;
						weaponInstace.Owner = gameObject;                   //���� ���� ����
						weaponInstace.SourcePrefab = newWeapon.gameObject;  //���� ������ ����� ������ ����
						weaponInstace.ShowWeapon(false);                    //���� ��Ȱ��

						weaponSlots[i] = weaponInstace;                     //���Կ� ������ weaponController �߰�
						return true;
					}
				}
			}
			else if (newWeapon.slotType == WeaponSlotType.Consum)
			{
				for (int i = mainWeaponIndex; i < consumedWeaponIndex; i++)
				{
					//����ִ� ����üũ�ؼ� �� ���Կ� ���� �߰�
					if (weaponSlots[i] == null)
					{
						WeaponController weaponInstace = Instantiate(newWeapon, weaponParentSocket);
						weaponInstace.transform.localPosition = Vector3.zero;
						weaponInstace.transform.localRotation = Quaternion.identity;
						weaponInstace.Owner = gameObject;                   //���� ���� ����
						weaponInstace.SourcePrefab = newWeapon.gameObject;  //���� ������ ����� ������ ����
						weaponInstace.ShowWeapon(false);                    //���� ��Ȱ��

						weaponSlots[i] = weaponInstace;                     //���Կ� ������ weaponController �߰�
						return true;
					}
				}
			}
			

			Debug.Log("������ �� á���ϴ�.");
			return false;
		}

		//�Ű������� ���� ���������� ������ ���Ⱑ������ ������ ���⸦ ��ȯ�޴� �Լ�
		public WeaponController HasWeapon(WeaponController weaponPrefab)
		{
			for (int i = 0; i < weaponSlots.Length; i++)
			{
				var weapon = weaponSlots[i];
				if (weapon != null && weapon.SourcePrefab == weaponPrefab.gameObject)
				{
					return weapon;
				}
			}

			//weaponPrefab�� ���� ������ ���Ⱑ ����
			return null;
		}

		//���� ��ü - ascendingOrder : ������������ ������, ������������ ������ bool���� ���� ȹ��
		//���� ����ִ¹��� false, ���ο� ���� true
		public void SwitchWeapon(bool ascendingOrder)
		{
			int newWeaponIndex = -1;
			int closestSlotDistance = weaponSlots.Length;   //�ִܰŸ��� ���ü� �ִ� �ִ밪

			for (int i = 0; i <= weaponSlots.Length; i++)
			{
				if (i != ActiveWeaponIndex && GetWeaponAtSlotIndex(i) != null)
				{
					int distanceToActiveIndex = GetDistanceBetweenWeaponSlots(ActiveWeaponIndex, i, ascendingOrder);
					if (distanceToActiveIndex < closestSlotDistance)
					{
						closestSlotDistance = distanceToActiveIndex;
						newWeaponIndex = i;
					}
				}
			}

			//���� ��ü ����
			SwitchToWeaponIndex(newWeaponIndex);
		}

		//�Ű������� ���� �ε�����, ��Ƽ�� ���� ��ü
		public void SwitchToWeaponIndex(int newWeaponIndex)
		{
			if (newWeaponIndex != ActiveWeaponIndex && newWeaponIndex >= 0)
			{
				weaponSwitchNewWeaponIndex = newWeaponIndex;

				weaponSwitchTimeStared = Time.time;

				//��Ƽ�� ������ ���� üũ
				if (GetActiveWeapon() == null)
				{
					//Ȱ��ȭ ���� ������� ���ο� ���� true
					weaponMainLocalPosition = downWeaponPosition.localPosition;  //�Ʒ����� ����
					weaponSwitchState = WeaponSwitchState.PutUpNew;

					ActiveWeaponIndex = weaponSwitchNewWeaponIndex;

					WeaponController weaponController = GetWeaponAtSlotIndex(ActiveWeaponIndex);

					OnSwitchToWeapon?.Invoke(weaponController);
				}
				else
				{
					//Ȱ��ȭ ���Ⱑ ������� �������� ������, ���ο�� �ø���
					weaponSwitchState = WeaponSwitchState.PutDownPrevious;

				}
			}

		}

		//��Ƽ�� ���� ���ϱ�
		public WeaponController GetActiveWeapon()
		{
			return GetWeaponAtSlotIndex(ActiveWeaponIndex);
		}

		//�ε����� ���� WeaponController�� ��ȯ�ϴ� �Լ�
		public WeaponController GetWeaponAtSlotIndex(int index)
		{
			//�ε����� ������ ���̺��� �۰�, 0���� Ŀ����
			if (index < weaponSlots.Length && index >= 0)
			{
				return weaponSlots[index];
			}

			return null;
		}

		//���� ����� Ȱ��ȭ Index�� ��ȯ�ϴ� �Լ�
		private int GetDistanceBetweenWeaponSlots(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
		{
			int distanceBetweenSlots = 0;

			if (ascendingOrder == true)
			{
				distanceBetweenSlots = toSlotIndex - fromSlotIndex;
			}
			else
			{
				distanceBetweenSlots = -1 * (toSlotIndex - fromSlotIndex);
			}

			//�Ÿ��� �����ϰ�� -> 0���Ϸ� ������ �������ų�, 8 �̻����� �ö�
			if (distanceBetweenSlots < 0)
			{
				distanceBetweenSlots += weaponSlots.Length;
			}

			return distanceBetweenSlots;
		}

		private void OnWeaponSwitch(WeaponController newWeapon)
		{
			if (newWeapon != null)
			{
				newWeapon.ShowWeapon(true);
			}
		}
	}
}