using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FpsSurvive.AI
{
    public class PatrollState : State
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

        //기본상태
        public override void OnUpdate(float deltaTime)
        {
            
        }
    }
}