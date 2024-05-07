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

        //Idle 상태 들어오기
        public override void OnEnter()
        {
            m_CharCtrl.Move(Vector3.zero);
            //애니메이터 설정

        }

        //기본상태
        public override void OnUpdate(float deltaTime)
        {
            if(enemy.Target)
            {
                if(enemy.IsAttackable)
                {
                    if(stateMachine.ElapseTime >= enemy.attackDelay)
                    {
                        //공격 상태 변경
                        stateMachine.ChangeState(new AttackState());
                    }
                }
                else
                {
                    //상태 변경
                    stateMachine.ChangeState(new ChaseState()); 
                }
            }
            
        }

        //상태 나가기 -> 나갈때 구현할 것 없음
        public override void OnExit()
        {
            
        }
    }
}