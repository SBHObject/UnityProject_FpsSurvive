using Cinemachine;
using FpsSurvive.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
		public CinemachineVirtualCamera virtualCamera;

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
		public Transform aimingWeaponPosition;

		private int weaponSwitchNewWeaponIndex;           //���� ��ü�� ���� �ε���
		[SerializeField]
		private float weaponSwitchDelay = 1f;           //
		private float weaponSwitchTimeStared = 0f;      //���� ��ü Ÿ�̸�(��ü��, �ٸ��ൿ ����)

		//�����϶� ���� ��鸲
		private float weaponBobFactor;
		[SerializeField]
		private float bobSharpness = 10f;
		[SerializeField]
		private float bobFrequency = 10f;
		private float defaultBobAmount = 0.05f;
		private float aimingBobAmount = 0.01f;

		private Vector3 weaponBobPosition;
		private Vector3 m_lastCharPosition;

		//�⺻ FOV��
		private float defaultFov = 60f;
		private float aimingFov;

		//���� ����
		public bool IsAiming { get; private set; }
		private float aimingAnimationSpeed = 10f;
		private bool isCamAiming = true;
		
		//���� �ݵ���
		private Vector3 weaponRecoilLocalPosition;
		private Vector3 accumulateRecoil;

		private float recoilSharpness = 50f;
		private float recoilReturnSharpness = 10f;

		private float recoilMaxDistance = 1f;

		//ī�޶� ���� �ݵ�
		public Transform baseCamLookPosition;
		public Transform camLookPosition;

		private Vector3 camRecoilLocalPosition;
		private Vector3 accumulateCamRecoil;

		private float camRecoilMaxDistance = 0.3f;
        private float camRecoilSharpness = 50f;
        private float camRecoilReturnSharpness = 2f;
        private float lastShootTime;
		private float firePerSecond;

		//���� ��ü�� ȣ��Ǵ� �̺�Ʈ �Լ�
		public UnityAction<WeaponController> OnSwitchToWeapon;

		public Image crossHairImage;
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

			//FOV �ʱ�ȭ
			SetFov(defaultFov);
			aimingFov = defaultFov;
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

			//���⸦ ����������
			if (weaponSwitchState == WeaponSwitchState.Up)
			{
				//����
				IsAiming = m_Input.aiming;

				//���
				bool hasFired = weaponContoller.HandleShootInput(m_Input.OnShootDown(), m_Input.OnShootHold());

				if(hasFired)
				{
					accumulateRecoil += weaponContoller.recoilForce * Vector3.back;
					accumulateRecoil = Vector3.ClampMagnitude(accumulateRecoil, recoilMaxDistance);

					accumulateCamRecoil += weaponContoller.camRecoilForce * Vector3.up;
					accumulateCamRecoil = Vector3.ClampMagnitude(accumulateCamRecoil, camRecoilMaxDistance);
				}

				//������
				if(m_Input.GetReloadInput())
				{
					weaponContoller.ReloadAnimationStart();
				}
			}
		}

		private void LateUpdate()
		{
			UpdateWeaponSwitching();
			WeaponBobing();
			WeaponAiming();
			WeaponRecoil();

			//����� ������ ���� ��ġ�� transform�� ����
			weaponParentSocket.localPosition = weaponMainLocalPosition + weaponBobPosition + weaponRecoilLocalPosition;
			camLookPosition.localPosition = Vector3.zero + camRecoilLocalPosition;
		}

		//����
		private void WeaponAiming()
		{
			if(weaponSwitchState != WeaponSwitchState.Up || isCamAiming == false)
			{
				return;
			}

			WeaponController nowWeapon = GetActiveWeapon();


			if((IsAiming && nowWeapon.IsReloading == false) && nowWeapon != null)
			{
				weaponMainLocalPosition = Vector3.Lerp(weaponMainLocalPosition, aimingWeaponPosition.localPosition + nowWeapon.aimingOffset,
					aimingAnimationSpeed * Time.deltaTime);

				aimingFov = Mathf.Lerp(aimingFov, defaultFov * (1 / nowWeapon.aimingFovRatio), aimingAnimationSpeed * Time.deltaTime);
				crossHairImage.enabled = false;
            }
			else
			{
				weaponMainLocalPosition = Vector3.Lerp(weaponMainLocalPosition, defaultWeaponPosition.localPosition,
					aimingAnimationSpeed * Time.deltaTime);

				aimingFov = Mathf.Lerp(aimingFov, defaultFov, aimingAnimationSpeed * Time.deltaTime);
				crossHairImage.enabled = true;
			}

			SetFov(aimingFov);
		}

		private void SetFov(float fov)
		{
			virtualCamera.m_Lens.FieldOfView = fov;
			weaponCam.fieldOfView = fov;
		}	

		//���� �ݵ�(�ڷ�)
		private void WeaponRecoil()
		{
			if(weaponRecoilLocalPosition.z >= accumulateRecoil.z * 0.99)
			{
				weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, accumulateRecoil, recoilSharpness * Time.deltaTime);
			}
			else
			{
				weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, Vector3.zero, recoilReturnSharpness * Time.deltaTime);
				accumulateRecoil = weaponRecoilLocalPosition;
			}

			if (camRecoilLocalPosition.y <= accumulateCamRecoil.y * 0.99)
			{
				camRecoilLocalPosition = Vector3.Lerp(camRecoilLocalPosition, accumulateCamRecoil, camRecoilSharpness * Time.deltaTime);
				lastShootTime = Time.time;
			}
			else
			{
				camRecoilLocalPosition = Vector3.Lerp(camRecoilLocalPosition, Vector3.zero, camRecoilReturnSharpness * Time.deltaTime);
				accumulateCamRecoil = camRecoilLocalPosition;
			}
		}

		//�̵��� ���� ���� ��鸲
		private void WeaponBobing()
		{
			if (Time.deltaTime > 0)
			{
				Vector3 playerVelocity = (m_PlayerMove.transform.position - m_lastCharPosition) / Time.deltaTime;
				float moveSpeed = m_Input.sprint ? m_PlayerMove.runSpeed : m_PlayerMove.walkSpeed;

				float charMoveFactor = 0f;
				if (m_PlayerMove.IsGrounded)
				{
					charMoveFactor = Mathf.Clamp01(playerVelocity.magnitude / moveSpeed);
				}

				weaponBobFactor = Mathf.Lerp(weaponBobFactor, charMoveFactor, bobSharpness * Time.deltaTime);

				float bobAmount = IsAiming? aimingBobAmount : defaultBobAmount;
				float frequency = bobFrequency;

				float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * weaponBobFactor;
				float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount * weaponBobFactor;

				weaponBobPosition = new Vector3(hBobValue, Mathf.Abs(vBobValue), 0);

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
			if (newWeapon == null)
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

						if (ActiveWeaponIndex == -1)
						{
							SwitchToWeaponIndex(0);
						}
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

						if (ActiveWeaponIndex == -1)
						{
							SwitchToWeaponIndex(3);
						}
						return true;
					}
				}
			}
			
			Debug.Log("������ �� á���ϴ�.");
			return false;
		}

		public bool AddWeapon(WeaponController newWeapon, int addIndex)
		{
			if (newWeapon == null)
			{
				Debug.Log("�߸��� ��û�Դϴ�");
				return false;
			}

			Destroy(weaponSlots[addIndex].gameObject);
			weaponSlots[addIndex] = null;

			if(newWeapon.slotType == WeaponSlotType.Main)
			{
				if (addIndex > mainWeaponIndex)
					return false;

				WeaponController weaponInstance = Instantiate(newWeapon, weaponParentSocket);
				weaponInstance.transform.localPosition = Vector3.zero;
				weaponInstance.transform.localRotation = Quaternion.identity;
				weaponInstance.Owner = gameObject;                   //���� ���� ����
				weaponInstance.SourcePrefab = newWeapon.gameObject;  //���� ������ ����� ������ ����
				weaponInstance.ShowWeapon(false);                    //���� ��Ȱ��
				weaponSlots[addIndex] = weaponInstance;                     //���Կ� ������ weaponController �߰�
				return true;
			}
			else if(newWeapon.slotType == WeaponSlotType.Consum)
			{
				if (addIndex < mainWeaponIndex || addIndex > consumedWeaponIndex)
					return false;

				WeaponController weaponInstance = Instantiate(newWeapon, weaponParentSocket);
				weaponInstance.transform.localPosition = Vector3.zero;
				weaponInstance.transform.localRotation = Quaternion.identity;
				weaponInstance.Owner = gameObject;                   //���� ���� ����
				weaponInstance.SourcePrefab = newWeapon.gameObject;  //���� ������ ����� ������ ����
				weaponInstance.ShowWeapon(false);                    //���� ��Ȱ��
				weaponSlots[addIndex] = weaponInstance;                     //���Կ� ������ weaponController �߰�
				return true;
			}

			return false;
		}

		public bool ChangeWeaponSlot(int targetIndex, int changedIndex)
		{
			if (weaponSlots[targetIndex].slotType != weaponSlots[changedIndex].slotType)
				return false;

			WeaponController tempSlot = weaponSlots[changedIndex];
			weaponSlots[changedIndex] = weaponSlots[targetIndex];
			weaponSlots[targetIndex] = tempSlot;
			return true;
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

					firePerSecond = weaponController.firePerSecond;

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