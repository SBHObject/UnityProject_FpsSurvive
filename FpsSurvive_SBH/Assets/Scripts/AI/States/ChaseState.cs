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
                //이동
                m_Agent.SetDestination(enemy.Target.position);
                m_CharCtrl.Move(m_Agent.velocity * deltaTime);

                //애니메이터 파라미터 설정

                if(m_Agent.remainingDistance <= m_Agent.stoppingDistance)
                {
                    //경로 계산중 여부, false일경우, 계산 완료
                    if (m_Agent.pathPending == false)
                    {
                        m_Agent.ResetPath();
                        stateMachine.ChangeState(new IdleState());  //공격상태 전환을 위한 idle State
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
            //애니메이션 파라미터 설정
        }
    }
}