using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FpsSurvive.Weapon
{
    public class ProjectileStandard : ProjectileBase
    {
        //�Ѿ� ���� ����
        public float detectRadius = 0.01f;
        //�Ѿ� ��ü
        public Transform root;
        //�Ѿ� ������ �߻��� ���κ�
        public Transform tip;

        //�Ѿ� ���� �ð�
        public float maxLifeTime = 4f;

        //�Ѿ� �ӵ�
        public float moveSpeed = 20f;

        //�Ѿ� �˵� ���� ����(�Ѿ��� ȭ�� �߾����� ���󰡵���), 0 �����ϰ�� �˵��� �������� ����
        public float trajectoryCorrentDistance = -1;

        public LayerMask hittableLayer = -1;

        //�˵� ������
        private ProjectileBase m_projectileBase;
        private Vector3 m_lastRootPos;
        private Vector3 m_velocity;
        private bool m_hasTrajectoryOverride;
        private Vector3 m_trajectoryCorrectionVector;
        private Vector3 m_consumedTrajectoryVector;
        List<Collider> m_ignoredColliders;

        private const QueryTriggerInteraction k_triggerInteraction = QueryTriggerInteraction.Collide;

		//������, ���� ������(��ź��) ���� ����

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
                        Debug.Log("����");
                        OnHit();
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

            //�浹 Ȯ��
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;

            Vector3 displacementSinceLastFrame = tip.position - m_lastRootPos;
            RaycastHit[] hits = Physics.SphereCastAll(m_lastRootPos, detectRadius,
                displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, hittableLayer, k_triggerInteraction);

            foreach(var hit in hits)
            {
                if(IsHitValid(hit)&& hit.distance < closestHit.distance)
                {
                    foundHit = true;
                    closestHit = hit;
                }
            }

            if(foundHit)
            {
                /*
                //���� ����(��ź�� ����Ʈ � ���)
                if(closestHit.distance <= 0)
                {
                    closestHit.point = root.position;
                    closestHit.normal = -transform.forward;
                }
                */

                Debug.Log("����");
                OnHit();
            }

            m_lastRootPos = root.position;
		}

		private bool IsHitValid(RaycastHit hit)
        {
            if(m_ignoredColliders != null && m_ignoredColliders.Contains(hit.collider))
            {
                return false;
            }

            return true;
        }

        private void OnHit()
        {
            Destroy(this.gameObject);
        }

	}
}