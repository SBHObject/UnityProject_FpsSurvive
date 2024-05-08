using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FpsSurvive.AI;
using UnityEngine.AI;
using FpsSurvive.Player;

namespace FpsSurvive.Game
{
    public class Enemy : MonoBehaviour
    {
        #region Variables
        
        //�������� �������, �������� �� ��� ���� Ÿ����
        private Health health;
        private bool forcedTarget = false;

        private IEnumerator targetCoroutine;

        [SerializeField]
        private float detectingRange = 5f;
        [SerializeField]
        private LayerMask targetMask;

        protected StateMachine stateMachine;
        protected NavMeshAgent m_Agent;
        protected Animator m_Animator;

        protected float m_RotateSpeed = 5.0f;
        protected float m_DelayTime = 0.1f;

        private Transform m_Target;
        public Transform Target => m_Target;

        private float m_DistanceToTarget;
        public float DistanceToTarget => m_DistanceToTarget;

        [HideInInspector]
        public virtual float AttackRange => 2.0f;

        public float attackDelay = 5f;

        public bool IsAttackable
        {
            get
            {
                if(Target)
                {
                    return (DistanceToTarget <= AttackRange);
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        protected virtual void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_Agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();

            m_Agent.updatePosition = false;
            m_Agent.updateRotation = true;

            stateMachine = new StateMachine(this, new IdleState());
            stateMachine.RegisterState(new ChaseState());
            stateMachine.RegisterState(new AttackState());
            stateMachine.RegisterState(new DeathState());

            health.OnDamaged += ForcedTargeting;
            //�� ã��
            targetCoroutine = UpdateTargetDelay(m_DelayTime);
            StartCoroutine(targetCoroutine);
        }

        protected virtual void Update()
        {
            //���¸ӽ� ������Ʈ
            stateMachine.Update(Time.deltaTime);

            if (!(stateMachine.CurrentState is ChaseState))
            {
                FaceToTarget();
            }

            if(forcedTarget && m_Target != null)
            {
                m_DistanceToTarget = Vector3.Distance(transform.position, m_Target.position);
            }
        }

        //��ġ ����
        private void OnAnimatorMove()
        {
            Vector3 position = m_Agent.nextPosition;
            m_Animator.rootPosition = position;
            transform.position = position;
        }

        public State ChangeState(State newState)
        {
            return stateMachine.ChangeState(newState);
        }

        IEnumerator UpdateTargetDelay(float delay)
        {
            while(true)
            {
                if (forcedTarget)
                {
                    StopCoroutine(targetCoroutine);
                }
				yield return new WaitForSeconds(delay);
                UpdateTarget();
            }
        }

        //���� ����� �� ã��
        private void UpdateTarget()
        {
            m_DistanceToTarget = 0;

            float shortestDistance = Mathf.Infinity;
            Transform nearestEnemy = null;

            Collider[] detectedEnemies = Physics.OverlapSphere(transform.position, detectingRange, targetMask);

            foreach(Collider e in detectedEnemies)
            {
                float distance = Vector3.Distance(transform.position, e.transform.position);
                if(distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = e.transform;
                }
            }

            if (forcedTarget == false)
            {
                //DetectingRange ���ο� ������ Ÿ��
                if (nearestEnemy != null && shortestDistance <= detectingRange)
                {
                    m_DistanceToTarget = shortestDistance;
                    m_Target = nearestEnemy;
                }
                else
                {
                    m_Target = null;
                }
            }
        }

        //�� �ٶ󺸱�
        private void FaceToTarget()
        {
            if (Target == null)
                return;
            Vector3 direction = (Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_RotateSpeed * Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectingRange);
        }

        private void ForcedTargeting(float damage, GameObject target)
        {
            if(forcedTarget == true)
            {
                return;
            }

            forcedTarget = true;
            m_Target = target.transform;
        }
    }
}