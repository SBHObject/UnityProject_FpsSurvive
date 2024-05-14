using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FpsSurvive.AI
{
    public class DeathState : State
    {
        private NavMeshAgent m_Agent;

        public override void OnInitialize()
        {
            m_Agent = enemy.GetComponent<NavMeshAgent>();

            m_Agent.SetDestination(enemy.transform.position);
        }

        public override void OnUpdate(float deltaTime)
        {
            
        }
    }
}