using Cinemachine;
using FpsSurvive.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FpsSurvive.Player
{
	//무기교체 상태
	public enum WeaponSwitchState
	{
		Up,                 //기본, 무기를 들고있는 상태
		Down,               //무기를 내려둔 상태(다음 무기로 교체하기 직전)
		PutDownPrevious,    //무기를 내리기 시작
		PutUpNew            //무기를 올리기 시작
	}

	public class PlayerWeaponManager : MonoBehaviour
    {
		#region Variables
		private PlayerNewInput m_Input;
		private PlayerMove m_PlayerMove;

		public Camera weaponCam;
		public CinemachineVirtualCamera virtualCamera;

		//무기가 장착되는 부모 오브젝트
		public Transform weaponParentSocket;

		//플레이어가 게임중에 들고다니는 무기 배열
		[SerializeField]
		private WeaponController[] weaponSlots = new WeaponController[5];
		//무기 슬롯 배열을 관리하는 인덱스
		public int ActiveWeaponIndex { get; private set; }

		private const int mainWeaponIndex = 3;
		private const int consumedWeaponIndex = 5;

		private WeaponSwitchState weaponSwitchState;        //무기 교체 상태
		private Vector3 weaponMainLocalPosition;        //무기 위치값(기본)

		public Transform defaultWeaponPosition;
		public Transform downWeaponPosition;
		public Transform aimingWeaponPosition;

		private int weaponSwitchNewWeaponIndex;           //새로 교체할 무기 인덱스
		[SerializeField]
		private float weaponSwitchDelay = 1f;           //
		private float weaponSwitchTimeStared = 0f;      //무기 교체 타이머(교체중, 다른행동 금지)

		//움직일때 무기 흔들림
		private float weaponBobFactor;
		[SerializeField]
		private float bobSharpness = 10f;
		[SerializeField]
		private float bobFrequency = 10f;
		private float defaultBobAmount = 0.05f;
		private float aimingBobAmount = 0.01f;

		private Vector3 weaponBobPosition;
		private Vector3 m_lastCharPosition;

		//기본 FOV값
		private float defaultFov = 60f;
		private float aimingFov;

		//무기 조준
		public bool IsAiming { get; private set; }
		private float aimingAnimationSpeed = 10f;
		private bool isCamAiming = true;
		
		//무기 반동값
		private Vector3 weaponRecoilLocalPosition;
		private Vector3 accumulateRecoil;

		private float recoilSharpness = 50f;
		private float recoilReturnSharpness = 10f;

		private float recoilMaxDistance = 1f;

		//카메라 무기 반동
		public Transform baseCamLookPosition;
		public Transform camLookPosition;

		private Vector3 camRecoilLocalPosition;
		private Vector3 accumulateCamRecoil;

		private float camRecoilMaxDistance = 0.3f;
        private float camRecoilSharpness = 50f;
        private float camRecoilReturnSharpness = 2f;
        private float lastShootTime;
		private float firePerSecond;

		//무기 교체시 호출되는 이벤트 함수
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
			//초기화
			ActiveWeaponIndex = -1;
			weaponSwitchState = WeaponSwitchState.Down;
			weaponSwitchTimeStared = Time.time;

			//이벤트함수 등록
			OnSwitchToWeapon += OnWeaponSwitch;

			//FOV 초기화
			SetFov(defaultFov);
			aimingFov = defaultFov;
		}

		private void Update()
		{
			WeaponController weaponContoller = GetActiveWeapon();

			//무기 교체 인풋
			if (weaponSwitchState == WeaponSwitchState.Up || weaponSwitchState == WeaponSwitchState.Down)
			{
				int switchInput = m_Input.GetSelectWeaponInput(); //이후 변경
				if (switchInput != 0)
				{
					if(GetWeaponAtSlotIndex(switchInput - 1) != null)
					{
						SwitchToWeaponIndex(switchInput - 1);
					}
				}
			}

			//무기를 들고있을경우
			if (weaponSwitchState == WeaponSwitchState.Up)
			{
				//조준
				IsAiming = m_Input.aiming;

				//사격
				bool hasFired = weaponContoller.HandleShootInput(m_Input.OnShootDown(), m_Input.OnShootHold());

				if(hasFired)
				{
					accumulateRecoil += weaponContoller.recoilForce * Vector3.back;
					accumulateRecoil = Vector3.ClampMagnitude(accumulateRecoil, recoilMaxDistance);

					accumulateCamRecoil += weaponContoller.camRecoilForce * Vector3.up;
					accumulateCamRecoil = Vector3.ClampMagnitude(accumulateCamRecoil, camRecoilMaxDistance);
				}

				//재장전
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

			//연산된 무기의 최종 위치를 transform에 적용
			weaponParentSocket.localPosition = weaponMainLocalPosition + weaponBobPosition + weaponRecoilLocalPosition;
			camLookPosition.localPosition = Vector3.zero + camRecoilLocalPosition;
		}

		//조준
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

		//무기 반동(뒤로)
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

		//이동에 따른 무기 흔들림
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

		//무기 상태에 따른 무기교체 연출, 무기 위치 연산 (weaponMainLoalPosition)
		private void UpdateWeaponSwitching()
		{
			//Lerp 변수 : 0 ~ 1 까지 갈 때, A -> B 로 변환
			float switchingTimeFactor = 0f;
			if (weaponSwitchDelay == 0f)
			{
				//1일 경우 B 상태 유지
				switchingTimeFactor = 1f;
			}
			else
			{
				//현재 시간 - 시작 시간 = 지나간 시간, Clamp를 통해 1이 넘으면 1로 고정
				switchingTimeFactor = Mathf.Clamp01((Time.time - weaponSwitchTimeStared) / weaponSwitchDelay);
			}

			//이동 연출 타이머 완료 후 상태처리
			if (switchingTimeFactor >= 1f)
			{
				if (weaponSwitchState == WeaponSwitchState.PutDownPrevious)
				{
					//현재 들고있는무기 false, 새로운무기 true
					WeaponController oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
					if (oldWeapon != null)
					{
						oldWeapon.ShowWeapon(false);
					}
					//기존무기 false 완료
					//새로운 무기를 Active 상태로 세팅
					ActiveWeaponIndex = weaponSwitchNewWeaponIndex;
					WeaponController newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
					OnSwitchToWeapon?.Invoke(newWeapon);

					//교체연출 준비
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

			//무기 이동 연출
			if (weaponSwitchState == WeaponSwitchState.PutDownPrevious) //위 -> 아래
			{
				//위에서 아래로 이동
				weaponMainLocalPosition = Vector3.Lerp(defaultWeaponPosition.localPosition, downWeaponPosition.localPosition, switchingTimeFactor);
			}
			else if (weaponSwitchState == WeaponSwitchState.PutUpNew)   //아래 -> 위
			{
				//아래에서 위로 이동
				weaponMainLocalPosition = Vector3.Lerp(downWeaponPosition.localPosition, defaultWeaponPosition.localPosition, switchingTimeFactor);
			}
		}

		//무기 추가
		public bool AddWeapon(WeaponController newWeapon)
		{
			if (newWeapon == null)
			{
				Debug.Log("같은 무기를 소지하고있거나, 잘못된 요청입니다");
				return false;
			}

			if (newWeapon.slotType == WeaponSlotType.Main)
			{
				for (int i = 0; i < mainWeaponIndex	; i++)
				{
					//비어있는 슬롯체크해서 빈 슬롯에 무기 추가
					if (weaponSlots[i] == null)
					{
						WeaponController weaponInstace = Instantiate(newWeapon, weaponParentSocket);
						weaponInstace.transform.localPosition = Vector3.zero;
						weaponInstace.transform.localRotation = Quaternion.identity;
						weaponInstace.Owner = gameObject;                   //무기 주인 세팅
						weaponInstace.SourcePrefab = newWeapon.gameObject;  //무기 생성시 사용한 프리팹 저장
						weaponInstace.ShowWeapon(false);                    //무기 비활성

						weaponSlots[i] = weaponInstace;                     //슬롯에 세팅한 weaponController 추가

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
					//비어있는 슬롯체크해서 빈 슬롯에 무기 추가
					if (weaponSlots[i] == null)
					{
						WeaponController weaponInstace = Instantiate(newWeapon, weaponParentSocket);
						weaponInstace.transform.localPosition = Vector3.zero;
						weaponInstace.transform.localRotation = Quaternion.identity;
						weaponInstace.Owner = gameObject;                   //무기 주인 세팅
						weaponInstace.SourcePrefab = newWeapon.gameObject;  //무기 생성시 사용한 프리팹 저장
						weaponInstace.ShowWeapon(false);                    //무기 비활성

						weaponSlots[i] = weaponInstace;                     //슬롯에 세팅한 weaponController 추가

						if (ActiveWeaponIndex == -1)
						{
							SwitchToWeaponIndex(3);
						}
						return true;
					}
				}
			}
			
			Debug.Log("슬롯이 꽉 찼습니다.");
			return false;
		}

		public bool AddWeapon(WeaponController newWeapon, int addIndex)
		{
			if (newWeapon == null)
			{
				Debug.Log("잘못된 요청입니다");
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
				weaponInstance.Owner = gameObject;                   //무기 주인 세팅
				weaponInstance.SourcePrefab = newWeapon.gameObject;  //무기 생성시 사용한 프리팹 저장
				weaponInstance.ShowWeapon(false);                    //무기 비활성
				weaponSlots[addIndex] = weaponInstance;                     //슬롯에 세팅한 weaponController 추가
				return true;
			}
			else if(newWeapon.slotType == WeaponSlotType.Consum)
			{
				if (addIndex < mainWeaponIndex || addIndex > consumedWeaponIndex)
					return false;

				WeaponController weaponInstance = Instantiate(newWeapon, weaponParentSocket);
				weaponInstance.transform.localPosition = Vector3.zero;
				weaponInstance.transform.localRotation = Quaternion.identity;
				weaponInstance.Owner = gameObject;                   //무기 주인 세팅
				weaponInstance.SourcePrefab = newWeapon.gameObject;  //무기 생성시 사용한 프리팹 저장
				weaponInstance.ShowWeapon(false);                    //무기 비활성
				weaponSlots[addIndex] = weaponInstance;                     //슬롯에 세팅한 weaponController 추가
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

		//매개변수로 들어온 프리팹으로 생성된 무기가있으면 생성된 무기를 반환받는 함수
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

			//weaponPrefab을 통해 생성된 무기가 없음
			return null;
		}

		//무기 교체 - ascendingOrder : 오름차순으로 가는지, 내림차순으로 가는지 bool형을 통해 획득
		//현재 들고있는무기 false, 새로운 무기 true
		public void SwitchWeapon(bool ascendingOrder)
		{
			int newWeaponIndex = -1;
			int closestSlotDistance = weaponSlots.Length;   //최단거리에 들어올수 있는 최대값

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

			//무기 교체 연출
			SwitchToWeaponIndex(newWeaponIndex);
		}

		//매개변수로 받은 인덱스로, 액티브 무기 교체
		public void SwitchToWeaponIndex(int newWeaponIndex)
		{
			if (newWeaponIndex != ActiveWeaponIndex && newWeaponIndex >= 0)
			{
				weaponSwitchNewWeaponIndex = newWeaponIndex;

				weaponSwitchTimeStared = Time.time;

				//액티브 무기의 유무 체크
				if (GetActiveWeapon() == null)
				{
					//활성화 무기 없을경우 새로운 무기 true
					weaponMainLocalPosition = downWeaponPosition.localPosition;  //아래에서 시작
					weaponSwitchState = WeaponSwitchState.PutUpNew;

					ActiveWeaponIndex = weaponSwitchNewWeaponIndex;

					WeaponController weaponController = GetWeaponAtSlotIndex(ActiveWeaponIndex);

					firePerSecond = weaponController.firePerSecond;

					OnSwitchToWeapon?.Invoke(weaponController);
				}
				else
				{
					//활성화 무기가 있을경우 기존무기 내리고, 새로운무기 올리기
					weaponSwitchState = WeaponSwitchState.PutDownPrevious;

				}
			}

		}

		//액티브 무기 구하기
		public WeaponController GetActiveWeapon()
		{
			return GetWeaponAtSlotIndex(ActiveWeaponIndex);
		}

		//인덱스를 통해 WeaponController를 반환하는 함수
		public WeaponController GetWeaponAtSlotIndex(int index)
		{
			//인덱스가 슬롯의 길이보다 작고, 0보다 커야함
			if (index < weaponSlots.Length && index >= 0)
			{
				return weaponSlots[index];
			}

			return null;
		}

		//가장 가까운 활성화 Index를 반환하는 함수
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

			//거리가 음수일경우 -> 0이하로 슬롯이 떨어지거나, 8 이상으로 올라감
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