using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



namespace FpsSurvive.Weapon
{
    public enum WeaponSlotType
    {
        Main,
        Consum
    }
    public enum WeaponShootType
    {
        Melee,
        Manual,
        Automatic
    }

    public class WeaponController : MonoBehaviour
    {
        public GameObject weaponRoot;
        public Transform weaponMuzzle;

        public WeaponSlotType slotType;
        public WeaponShootType shootType;

        //펠릿의 수, 기본적으로 1발, 샷건계열에서 여러발
        public int bulletPerShoot = 1;
        //초당 발사수
        public float firePerSecond = 1;
        //사격 가능 여부
        public bool isFire = false;

		public float BulletSpreadAngle = 0f;

        public float aimingFovRatio;    //조준시 배율 (기본FOV * aimingFovRatio)

		//반동(밀려날 힘의 크기)
		[Range(0f, 2f)]
        public float recoilForce = 1;

        [Header("AmmoParameter")]
        public GameObject shellMesh;    //탄피
        public Transform ejectionPort;  //탄피 배출구 위치

        [Range(0f, 5f)]
        public float shellEjectionForce = 2f;   //탄피 배출시 속도?힘?
        [Range(0, 30)]
        public int shellPoolSize = 1;   //탄피 오브젝트 풀 최대치

        public int clipSize = 30;       //탄창 크기
        public int maxAmmo = 150;        //최대휴행탄수

        public UnityAction OnShoot;

        private int m_CarryedBullets;
        private int m_CurrentAmmo;
        private bool m_WantToShot;

        private bool isWeaponActive = false;

        //총알
        public ProjectileBase projectile;

        public GameObject Owner { get; set; }

        public GameObject SourcePrefab { get; set; }

        public Vector3 muzzleWorldVelocity { get; private set; }
        private Vector3 lastMuzzlePosition;

        public bool IsWeaponActive { get; private set; }

        public int GetCarriedBullets() => m_CarryedBullets;
        public int GetCurrentAmmo() => m_CurrentAmmo;

        public bool IsReloading { get; private set; }

        private Queue<Rigidbody> m_AmmoPool;

		private void Awake()
		{
            m_CurrentAmmo = clipSize;
            m_CarryedBullets = maxAmmo;
            lastMuzzlePosition = weaponMuzzle.position;
		}

        void Update()
        {
            if(Time.deltaTime > 0)
            {
                muzzleWorldVelocity = (weaponMuzzle.position - lastMuzzlePosition) / Time.deltaTime;
                lastMuzzlePosition = weaponMuzzle.position;
            }
        }

        public bool HandleShootInput(bool inputDown, bool inputHold)
        {
            m_WantToShot = inputDown || inputHold;
            switch(shootType)
            {
                case WeaponShootType.Manual:
                    if(inputDown)
                    {
                        return TryShoot();
                    }
                    return false;
                case WeaponShootType.Automatic:
                    if(inputHold)
                    {
                        return TryShoot();
                    }
                    return false;
                case WeaponShootType.Melee:
                    if (inputDown)
                    {
                        return true;
                    }

                    return false;

                default:
                    return false;
            }
        }

        private bool TryShoot()
        {
            if (CanFireCheck())
            {
                StartCoroutine(fireWait(firePerSecond));
                HendleShoot();
                m_CurrentAmmo -= 1;
                return true;
            }
            return false;
        }
        
        private void HendleShoot()
        {
            Debug.Log("사격");
            //총알 생성
            for(int i = 0; i < bulletPerShoot; i++)
            {
                Vector3 shotDirection = GetShotDirectionWithinSpread(weaponMuzzle);
                ProjectileBase newProjectile = Instantiate(projectile, weaponMuzzle.position, Quaternion.LookRotation(shotDirection));
                newProjectile.Shoot(this);
            }

            OnShoot?.Invoke();

            //발사 이펙트

            //탄피 배출

            //발사 사운드, 애니매이션 등
        }
        
        private bool CanFireCheck()
        {
            if(isFire == true || m_CurrentAmmo <= 0)
            {
                if(m_CurrentAmmo <= 0)
                {
                    Debug.Log("총알부족");
                }
                return false;
            }

            return true;
        }

        private IEnumerator fireWait(float rps)
        {
            isFire = true;
            yield return new WaitForSeconds(1/rps);
			isFire = false;
		}

        private Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
        {
            float spreadAngleRatio = BulletSpreadAngle / 180f;
            Vector3 spreadDirection = Vector3.Slerp(shootTransform.forward, Random.insideUnitSphere, spreadAngleRatio);
            return spreadDirection;
        }

        public void ShowWeapon(bool show)
        {
            weaponRoot.SetActive(show);

            isWeaponActive = show;
        }
	}
}