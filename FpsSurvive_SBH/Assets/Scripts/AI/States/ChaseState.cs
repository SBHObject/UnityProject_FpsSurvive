using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FpsSurvive.AI
{
    public class ChaseState : State
    {
        private Animator m_Animator;
        private CharacterController m_CharCtrl;
        private NavMeshAgent m_Agent;

        public override void OnInitialize()
        {
            m_Animator = enemy.GetComponent<Animator>();
            m_CharCtrl = enemy.GetComponent<CharacterController>();
            m_Agent = enemy.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            if(m_Agent)
            {
                m_Agent.SetDestination(enemy.Target.position);
                m_Agent.stoppingDistance = enemy.AttackRange;
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            if(enemy.Target)
            {
                //�̵�
                m_Agent.SetDestination(enemy.Target.position);
                m_CharCtrl.Move(m_Agent.velocity * deltaTime);

                //�ִϸ����� �Ķ���� ����

                if(m_Agent.remainingDistance <= m_Agent.stoppingDistance)
                {
                    //��� ����� ����, false�ϰ��, ��� �Ϸ�
                    if (m_Agent.pathPending == false)
                    {
                        m_Agent.ResetPath();
                        stateMachine.ChangeState(new IdleState());  //���ݻ��� ��ȯ�� ���� idle State
                    }
                }
            }
            else
            {
                m_Agent.ResetPath();
                stateMachine.ChangeState(new IdleState());
            }
        }

        public override void OnExit()
        {
            enemy.attackDelay = 0;
            //�ִϸ��̼� �Ķ���� ����
        }
    }
}