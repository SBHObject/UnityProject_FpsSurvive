using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Weapon
{
    public class ProjectileThrow : ProjectileBase
    {
		#region Variables
		private Rigidbody rigidbody;
		private ProjectileBase projectileBase;

		public Transform localThrowVector;

		//감지 (닿으면 기믹 작동)
		[SerializeField]
		private float detectRange = 0.5f;   //콜라이더 감지 범위

		public Transform center;

		[SerializeField]
		private float fuseLifeTime = -1;    //-1일경우, 신관이 없음(고유 작동 기믹)
		[SerializeField]
		private float throwForce = 20f;
		
		public LayerMask hittableLayer = -1;    //즉시 작동
		#endregion

		private void OnEnable()
		{
			rigidbody = GetComponent<Rigidbody>();
			projectileBase = GetComponent<ProjectileBase>();

			projectileBase.OnShoot += OnShoot;

			if(fuseLifeTime >= 0)
			{
				Destroy(gameObject, fuseLifeTime + 0.01f);
			}
		}

		private new void OnShoot()
		{
			rigidbody.AddForce(localThrowVector.localPosition, ForceMode.Impulse);
		}

		private void Update()
		{
			
		}

	}
}