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
            //�ִϸ����� ��������
            m_Animator = enemy.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            //���� Ʈ���� �۵�
            Debug.Log("����");
        }

        public override void OnUpdate(float deltaTime)
        {
            if (stateMachine.ElapseTime > 1f)
            {
                Debug.Log("���� ����");
                stateMachine.ChangeState(new IdleState());
            }
        }

        public override void OnExit()
        {
            enemy.attackDelay = m_AttackDelay;
        }
    }
}