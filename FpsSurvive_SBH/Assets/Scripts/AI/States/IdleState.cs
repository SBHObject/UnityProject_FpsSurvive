using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FpsSurvive.AI
{
    public class IdleState : State
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

        //Idle ���� ������
        public override void OnEnter()
        {
            m_CharCtrl.Move(Vector3.zero);
            //�ִϸ����� ����

        }

        //�⺻����
        public override void OnUpdate(float deltaTime)
        {
            if(enemy.Target)
            {
                if(enemy.IsAttackable)
                {
                    if(stateMachine.ElapseTime >= enemy.attackDelay)
                    {
                        //���� ���� ����
                        stateMachine.ChangeState(new AttackState());
                    }
                }
                else
                {
                    //���� ����
                    stateMachine.ChangeState(new ChaseState()); 
                }
            }
            
        }

        //���� ������ -> ������ ������ �� ����
        public override void OnExit()
        {
            
        }
    }
}