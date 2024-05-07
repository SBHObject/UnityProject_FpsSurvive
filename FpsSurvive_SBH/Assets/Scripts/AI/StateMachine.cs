using FpsSurvive.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.AI
{
    public abstract class State
    {
        protected StateMachine stateMachine;    //현재 상태를 관리하는 상태머신
        protected Enemy enemy;                  //상태머신을 가지고있는 Enemy

        //생성자
        public State() { }

        //State를 세팅 : stateMachine, enemy
        public void SetState(StateMachine _stateMachine, Enemy _enemy)
        {
            this.stateMachine = _stateMachine;
            this.enemy = _enemy;

            //상태 초기화
            OnInitialize();
        }

        //상태 초기화 함수
        public virtual void OnInitialize()
        {

        }

        //상태 들어가기 (1회 호출)
        public virtual void OnEnter()
        {

        }

        public abstract void OnUpdate(float deltaTime); //추상메서드

        //상태 나오기(1회 호출)
        public virtual void OnExit()
        {

        }
    }

    public class StateMachine
    {
        private Enemy enemy;    //상태머신을 가진 Enemy(부모 클래스)

        private State m_CurrentState;   //현재 상태
        public State CurrentState => m_CurrentState;

        private State m_PreviousState;  //이전 상태
        public State PreviousState => m_PreviousState;

        private float m_ElapseTime = 0;   //현재상태에 진행된 누적 시간 카운팅
        public float ElapseTime => m_ElapseTime;

        //상태 저장
        private Dictionary<System.Type, State> states = new Dictionary<System.Type, State>();

        //생성자
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
            //상태 세팅
            state.SetState(this, enemy);
            //상태 등록
            states[state.GetType()] = state;
        }

        //상태 업데이트
        public void Update(float deltaTime)
        {
            m_ElapseTime += deltaTime;
            m_CurrentState.OnUpdate(deltaTime);
        }

        //상태 변경
        public State ChangeState(State newState)
        {
            //현재상태 체크
            var newType = newState.GetType();
            if(newType == m_CurrentState?.GetType())
            {
                return m_CurrentState;
            }

            //현재상태에서 나오기
            if(m_CurrentState != null)
            {
                m_CurrentState.OnExit();
            }

            m_PreviousState = m_CurrentState;
            m_CurrentState = states[newType];

            //상태 들어가기
            m_CurrentState.OnEnter();
            m_ElapseTime = 0f;

            return m_CurrentState;
        }
    }
}