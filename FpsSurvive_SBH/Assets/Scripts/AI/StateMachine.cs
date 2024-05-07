using FpsSurvive.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.AI
{
    public abstract class State
    {
        protected StateMachine stateMachine;    //���� ���¸� �����ϴ� ���¸ӽ�
        protected Enemy enemy;                  //���¸ӽ��� �������ִ� Enemy

        //������
        public State() { }

        //State�� ���� : stateMachine, enemy
        public void SetState(StateMachine _stateMachine, Enemy _enemy)
        {
            this.stateMachine = _stateMachine;
            this.enemy = _enemy;

            //���� �ʱ�ȭ
            OnInitialize();
        }

        //���� �ʱ�ȭ �Լ�
        public virtual void OnInitialize()
        {

        }

        //���� ���� (1ȸ ȣ��)
        public virtual void OnEnter()
        {

        }

        public abstract void OnUpdate(float deltaTime); //�߻�޼���

        //���� ������(1ȸ ȣ��)
        public virtual void OnExit()
        {

        }
    }

    public class StateMachine
    {
        private Enemy enemy;    //���¸ӽ��� ���� Enemy(�θ� Ŭ����)

        private State m_CurrentState;   //���� ����
        public State CurrentState => m_CurrentState;

        private State m_PreviousState;  //���� ����
        public State PreviousState => m_PreviousState;

        private float m_ElapseTime = 0;   //������¿� ����� ���� �ð� ī����
        public float ElapseTime => m_ElapseTime;

        //���� ����
        private Dictionary<System.Type, State> states = new Dictionary<System.Type, State>();

        //������
        public StateMachine(Enemy _enemy, State initialState)
        {
            enemy = _enemy;

            RegisterState(initialState);

            m_CurrentState = initialState; 
            m_CurrentState.OnEnter();
            m_ElapseTime = 0f;
        }

        public void RegisterState(State state)
        {
            //���� ����
            state.SetState(this, enemy);
            //���� ���
            states[state.GetType()] = state;
        }

        //���� ������Ʈ
        public void Update(float deltaTime)
        {
            m_ElapseTime += deltaTime;
            m_CurrentState.OnUpdate(deltaTime);
        }

        //���� ����
        public State ChangeState(State newState)
        {
            //������� üũ
            var newType = newState.GetType();
            if(newType == m_CurrentState?.GetType())
            {
                return m_CurrentState;
            }

            //������¿��� ������
            if(m_CurrentState != null)
            {
                m_CurrentState.OnExit();
            }

            m_PreviousState = m_CurrentState;
            m_CurrentState = states[newType];

            //���� ����
            m_CurrentState.OnEnter();
            m_ElapseTime = 0f;

            return m_CurrentState;
        }
    }
}