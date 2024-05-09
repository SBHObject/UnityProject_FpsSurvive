using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FpsSurvive.AnimationParameter;

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

    [RequireComponent(typeof(AudioSource))]
    public class WeaponController : MonoBehaviour
    {
        #region Variables
        public GameObject weaponRoot;
        public Transform weaponMuzzle;

        public WeaponSlotType slotType;
        public WeaponShootType shootType;

        //�縴�� ��, �⺻������ 1��, ���ǰ迭���� ������
        public int bulletPerShoot = 1;
        //�ʴ� �߻��
        public float firePerSecond = 1;
        //��� ���� ����
        public bool isFire = false;

		public float BulletSpreadAngle = 0f;

        public float aimingFovRatio;    //���ؽ� ���� (�⺻FOV * aimingFovRatio)
        public Vector3 aimingOffset = Vector3.zero; //���ؽ� ��ġ �߰�����


        //�ݵ�(�з��� ���� ũ��)
        [Range(0f, 2f)]
        public float recoilForce = 1;

        public float camRecoilForce = 0.1f;

        [Header("AmmoParameter")]
        public GameObject shellMesh;    //ź��
        public Transform ejectionPort;  //ź�� ���ⱸ ��ġ

        [Range(0f, 5f)]
        public float shellEjectionForce = 2f;   //ź�� ����� �ӵ�?��?
        [Range(0, 30)]
        public int shellPoolSize = 1;   //ź�� ������Ʈ Ǯ �ִ�ġ

        public int clipSize = 30;       //źâ ũ��
        public int maxAmmo = 150;        //�ִ�����ź�� - ���� �κ��丮�� �����Ұ�...

        public float gunDamage = 20;

        public UnityAction OnShoot;

        private int m_CarryedBullets;
        private int m_CurrentAmmo;
        private bool m_WantToShot;

        private bool isWeaponActive = false;

        //�Ѿ�
        public ProjectileBase projectile;

        public GameObject Owner { get; set; }

        public GameObject SourcePrefab { get; set; }

        public Vector3 muzzleWorldVelocity { get; private set; }
        private Vector3 lastMuzzlePosition;

        public bool IsWeaponActive { get; private set; }

        public int GetCarriedBullets() => m_CarryedBullets;
        public int GetCurrentAmmo() => m_CurrentAmmo;

        public bool IsReloading { get; private set; }
        [SerializeField] private int reloadAmount;

        private Queue<Rigidbody> m_AmmoPool;

        //SFX ����
        public AudioClip shootSfx;
        public AudioClip changeSfx;
        public AudioClip reloadingSfx;
        private AudioSource m_AudioSource;

        //VFX ����
        public GameObject shootVfx;
        private Animator ani;

        #endregion
        private void Awake()
		{
            m_CurrentAmmo = clipSize;
            m_CarryedBullets = maxAmmo;
            lastMuzzlePosition = weaponMuzzle.position;
            m_AudioSource = GetComponent<AudioSource>();
            ani = GetComponent<Animator>();
		}

        private void OnEnable()
        {
            if (changeSfx)
            {
                m_AudioSource.PlayOneShot(changeSfx);
            }
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
                return true;
            }
            return false;
        }
        
        private void HendleShoot()
        {
            //�Ѿ� ����
            for(int i = 0; i < bulletPerShoot; i++)
            {
                Vector3 shotDirection = GetShotDirectionWithinSpread(weaponMuzzle);
                ProjectileBase newProjectile = Instantiate(projectile, weaponMuzzle.position, Quaternion.LookRotation(shotDirection));
                newProjectile.Shoot(this);
            }

            OnShoot?.Invoke();

            //ź�� ����

            //�߻� ����Ʈ
            if(shootVfx != null)
            {
                shootVfx.SetActive(true);
            }

            //�߻� ����, �ִϸ��̼� ��
            if(shootSfx != null)
            {
                m_AudioSource.PlayOneShot(shootSfx);
            }

            ani.SetTrigger(AniParameters.isShoot);

            if(slotType == WeaponSlotType.Consum)
            {
                Reload();
            }
        }
        
        private bool CanFireCheck()
        {
            if(isFire == true || IsReloading == true)
            {
                return false;
            }

            if (m_CurrentAmmo <= 0)
            {
                ReloadAnimationStart();
                return false;
            }

            return true;
        }

        private IEnumerator fireWait(float rps)
        {
            isFire = true;
            m_CurrentAmmo -= 1;
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

        //����, ������ �ִϸ������� �� ��� �ִϸ����� ��û�� ���ǰɰ�, ���� ������ ���� ����, �ִϸ����Ϳ��� ȣ��
        public void Reload()
        {
            int usedAmmo = reloadAmount - m_CurrentAmmo;
            int realUseAmmo = m_CarryedBullets - usedAmmo;

            realUseAmmo = realUseAmmo >= 0 ? usedAmmo : m_CarryedBullets;

            m_CurrentAmmo = Mathf.Min(m_CarryedBullets, reloadAmount);
            m_CarryedBullets -= realUseAmmo;

            IsReloading = false;
        }

        public void ReloadAnimationStart()
        {
            if (m_CurrentAmmo == clipSize || m_CarryedBullets <= 0 || IsReloading == true)
            {
                return;
            }

            if (reloadingSfx != null)
            {
                m_AudioSource.PlayOneShot(reloadingSfx);
            }

            ani.SetTrigger(AniParameters.isReloading);

            IsReloading = true;
        }
	}
}