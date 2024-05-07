using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    public class PlayerCamRootMove : MonoBehaviour
    {
        public Transform lookPosition;

		private void Update()
		{
			transform.LookAt(lookPosition);
		}
	}
}