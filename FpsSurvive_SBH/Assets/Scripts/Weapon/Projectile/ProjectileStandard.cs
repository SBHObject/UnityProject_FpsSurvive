using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FpsSurvive.Game;
using FpsSurvive.GamePlay;

namespace FpsSurvive.Weapon
{
    public class ProjectileStandard : ProjectileBase
    {
        //총알 판정 범위
        public float detectRadius = 0.01f;
        //총알 본체
        public Transform root;
        //총알 판정이 발생할 끝부분
        public Transform tip;

        //총알 유지 시간
        public float maxLifeTime = 4f;

        //총알 속도
        public float moveSpeed = 20f;

        //총알 궤도 수정 변수(총알이 화면 중앙으로 날라가도록), 0 이하일경우 궤도를 수정하지 않음
        public float trajectoryCorrentDistance = -1;

        public LayerMask hittableLayer = -1;

        //궤도 수정용
        private ProjectileBase m_projectileBase;
        private Vector3 m_lastRootPos;
        private Vector3 m_velocity;
        private bool m_hasTrajectoryOverride;
        private Vector3 m_trajectoryCorrectionVector;
        private Vector3 m_consumedTrajectoryVector;
        List<Collider> m_ignoredColliders;

        private const QueryTriggerInteraction k_triggerInteraction = QueryTriggerInteraction.Collide;

        [Tooltip("Max Range")]
        //사거리 제한 무기
        [SerializeField]
        private bool isRangedLostDamage = false;
        [SerializeField]
        private AnimationCurve rangeLostDamage;
        [SerializeField]
        private float maxCanDamageDistance = 1000f;

        //VFX
        private float effectOffset = 0.01f;
        public GameObject[] hitEffects;

        //SFX

		//데미지, 범위 데미지(유탄류) 이후 생성

		private void OnEnable()
		{
            m_projectileBase = GetComponent<ProjectileBase>();
            m_projectileBase.OnShoot += OnShoot;

            Destroy(gameObject, maxLifeTime);
		}

        private new void OnShoot()
        {
            m_lastRootPos = root.position;
            m_velocity = transform.forward * moveSpeed;
            m_ignoredColliders = new List<Collider>();
            transform.position += m_projectileBase.InheritedMuzzleVelocity * Time.deltaTime;

            Collider[] ownerCollider = m_projectileBase.Owner.GetComponentsInChildren<Collider>();
            m_ignoredColliders.AddRange(ownerCollider);

            PlayerWeaponManager playerWeaponManager = m_projectileBase.Owner.GetComponent<PlayerWeaponManager>();

            if(playerWeaponManager)
            {
                m_hasTrajectoryOverride = true;

                Vector3 camToMuzzle = (m_projectileBase.InitialPosition - playerWeaponManager.weaponCam.transform.position);

                m_trajectoryCorrectionVector = Vector3.ProjectOnPlane(-camToMuzzle, playerWeaponManager.weaponCam.transform.forward);

                if(trajectoryCorrentDistance == 0)
                {
                    transform.position += m_trajectoryCorrectionVector;
                    m_consumedTrajectoryVector = m_trajectoryCorrectionVector;
                }
                else if(trajectoryCorrentDistance < 0)
                {
                    m_hasTrajectoryOverride = false;
                }

                if (Physics.Raycast(playerWeaponManager.weaponCam.transform.position, camToMuzzle.normalized, out RaycastHit hit,
                    camToMuzzle.magnitude, hittableLayer, k_triggerInteraction))
                {
                    if(IsHitValid(hit))
                    {
                        Debug.Log("명중");
                        OnHit(hit.point, hit.normal, hit.collider);
                    }
                }

			}
        }

		private void Update()
		{
			transform.position += m_velocity * Time.deltaTime;

            if(m_hasTrajectoryOverride && m_consumedTrajectoryVector.sqrMagnitude < m_trajectoryCorrectionVector.sqrMagnitude)
            {
                Vector3 correctionLeft = m_trajectoryCorrectionVector - m_consumedTrajectoryVector;
                float distanceThisFrame = (root.position - m_lastRootPos).magnitude;
                Vector3 correctionThisFrame = (distanceThisFrame / trajectoryCorrentDistance) * m_trajectoryCorrectionVector;
                correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
                m_consumedTrajectoryVector += correctionThisFrame;

                if(m_consumedTrajectoryVector.sqrMagnitude == m_trajectoryCorrectionVector.sqrMagnitude)
                {
                    m_hasTrajectoryOverride = false;
                }

                transform.position += correctionThisFrame;
            }

            transform.forward = m_velocity.normalized;

            //충돌 확인
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;

            Vector3 displacementSinceLastFrame = tip.position - m_lastRootPos;
            RaycastHit[] hits = Physics.SphereCastAll(m_lastRootPos, detectRadius,
                displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, hittableLayer, k_triggerInteraction);

            foreach(var hit in hits)
            {
                if(hit.distance < closestHit.distance)
                {
                    foundHit = IsHitValid(hit);
                    closestHit = hit;
                }
            }

            if(foundHit)
            {
                OnHit(closestHit.point, closestHit.normal, closestHit.collider);
            }

            m_lastRootPos = root.position;
		}

		private bool IsHitValid(RaycastHit hit)
        {
            if(hit.collider.isTrigger && hit.collider.GetComponent<Damageable>() == null)
            {
                return false;
            }

            if(m_ignoredColliders != null && m_ignoredColliders.Contains(hit.collider))
            {
                return false;
            }

            return true;
        }

        private void OnHit(Vector3 hitPoint, Vector3 normal, Collider collider)
        {
            Damageable damageable = collider.GetComponent<Damageable>();
            if(damageable != null)
            {
                damageable.InflictDamage(FinalDamage(), false, Owner);
            }

            //피격 파티클 생성
            HitParticleInfo particleInfo = collider.GetComponentInParent<HitParticleInfo>();

			if (particleInfo != null && particleInfo.material != HitMaterial.None)
            {
                GameObject instantiateParticle = hitEffects[(int)particleInfo.material];
                GameObject hitEffect = Instantiate(instantiateParticle, hitPoint + effectOffset * normal, Quaternion.LookRotation(normal));
                Destroy(hitEffect, 5f);
            }

            Destroy(this.gameObject);
        }

        private float FinalDamage()
        {
            float finalDamage = Damage;
            if(isRangedLostDamage)
            {
                float flyingAmount = Vector3.Distance(transform.position, InitialPosition);
                finalDamage = finalDamage * rangeLostDamage.Evaluate(flyingAmount / maxCanDamageDistance);
            }

            return finalDamage;
        }
	}
}