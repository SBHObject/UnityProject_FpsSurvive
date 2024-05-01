using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Weapon
{
    public class SelfActiveFalse : MonoBehaviour
    {
        [SerializeField]
        private float DeActiveTimer = 0.5f;
        private float startTime;

        private void OnEnable()
        {
            startTime = Time.time;
        }

        private void Update()
        {
            if(Time.time >= startTime + DeActiveTimer)
            {
                gameObject.SetActive(false);
            }
        }
    }
}