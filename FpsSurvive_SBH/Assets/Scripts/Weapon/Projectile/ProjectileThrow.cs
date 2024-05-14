using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Weapon
{
    public class ProjectileThrow : ProjectileBase
    {
		#region Variables
		private Rigidbody rb;
		private ProjectileBase projectileBase;

		public Transform localThrowVector;
		public Collider hitBox;

		//중복데미지 방지...
		private bool isActive = false;

		//감지 (닿으면 기믹 작동)
		[SerializeField]
		private float detectRange = 0.5f;   //콜라이더 감지 범위

		public Transform center;

		[SerializeField]
		private float fuseLifeTime = -1;    //-1일경우, 신관이 없음(고유 작동 기믹)
		private float initialTime;

		[SerializeField]
		private float throwForce = 20f;

		public LayerMask hittableLayer = -1;    //즉시 작동

		private AreaDamage areaDamage;

		//VFX
		[SerializeField]
		private float effectOffest = 0f;
		public GameObject HitEffect;

		private float effectDestroyTime = 3f;
		//Sfx
		#endregion

		private void OnEnable()
		{
			rb = GetComponent<Rigidbody>();
			projectileBase = GetComponent<ProjectileBase>();
			areaDamage = GetComponent<AreaDamage>();
			initialTime = Time.time;

			projectileBase.OnShoot += OnShoot;

		}

		private void Update()
		{
			RaycastHit closestHit = new RaycastHit();
			closestHit.distance = Mathf.Infinity;

			bool foundHit = false;

			RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectRange, transform.forward, 1f, hittableLayer, QueryTriggerInteraction.Ignore);

			foreach(var hit in hits)
			{
				if(hit.distance < closestHit.distance)
				{
					foundHit = true;
					closestHit = hit;
				}
			}

			if(foundHit && fuseLifeTime < 0 && isActive == false)
			{
				isActive = true;
				OnHit(closestHit.normal);
			}

			if (fuseLifeTime >= 0 && initialTime + fuseLifeTime <= Time.time && isActive == false)
			{
				isActive = true;
				OnHit(closestHit.normal);
			}
		}

		private new void OnShoot()
		{
			rb.AddForce((transform.forward + Vector3.up) * throwForce, ForceMode.Impulse);
		}

		

		private void OnHit(Vector3 normal)
		{
			areaDamage.InflictAreaDamage(Damage, transform.position, hittableLayer, QueryTriggerInteraction.Collide, projectileBase.Owner);

			GameObject effect = Instantiate(HitEffect, transform.position + effectOffest * normal, Quaternion.LookRotation(normal));
			Destroy(effect, effectDestroyTime);

            Destroy(gameObject);
        }
	}
}