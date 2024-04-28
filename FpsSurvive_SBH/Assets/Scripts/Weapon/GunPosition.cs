using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Weapon
{
    public class GunPosition : MonoBehaviour
    {
        #region Variables
        public Transform playerGunPos;

		#endregion

        // Update is called once per frame
        void Update()
        {
            transform.position = playerGunPos.position;
            transform.rotation = playerGunPos.rotation;
        }
    }
}