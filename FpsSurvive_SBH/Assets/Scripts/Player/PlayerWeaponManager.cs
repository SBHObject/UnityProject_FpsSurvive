using FpsSurvive.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

		//유저에게 처음 지급하는 무기 리스트 - 프리팹 리스트
		public List<WeaponController> startingWeapons = new List<WeaponController>();

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

		private int weaponSwitchNewWeaponIndex;           //새로 교체할 무기 인덱스
		[SerializeField]
		private float weaponSwitchDelay = 1f;           //
		private float weaponSwitchTimeStared = 0f;      //무기 교체 타이머(교체중, 다른행동 금지)

		private Vector3 m_lastCharPosition;

		//무기 교체시 호출되는 이벤트 함수
		public UnityAction<WeaponController> OnSwitchToWeapon;
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

			//시작무기 지급
			foreach (var weapon in startingWeapons)
			{
				AddWeapon(weapon);
			}

			SwitchWeapon(true);
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

			if (weaponSwitchState == WeaponSwitchState.Up)
			{
				bool hasFired = weaponContoller.HandleShootInput(m_Input.OnShootDown(), m_Input.OnShootHold());
			}
		}

		private void LateUpdate()
		{
			UpdateWeaponSwitching();

			//연산된 무기의 최종 위치를 transform에 적용
			weaponParentSocket.localPosition = weaponMainLocalPosition;
		}

		//이동에 따른 무기 흔들림
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
			if (newWeapon == null || HasWeapon(newWeapon) != null)
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
						return true;
					}
				}
			}
			

			Debug.Log("슬롯이 꽉 찼습니다.");
			return false;
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