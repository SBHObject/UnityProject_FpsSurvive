using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FpsSurvive.AI
{
    public class AttackState : State
    {
        private Animator m_Animator;

        protected float m_AttackDelay = 1f;

        public override void OnInitialize()
        {
            //애니메이터 가져오기
            m_Animator = enemy.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            //공격 트리거 작동
            Debug.Log("공격");
        }

        public override void OnUpdate(float deltaTime)
        {
            if (stateMachine.ElapseTime > 1f)
            {
                Debug.Log("공격 종료");
                stateMachine.ChangeState(new IdleState());
            }
        }

        public override void OnExit()
        {
            enemy.attackDelay = m_AttackDelay;
        }
    }
}