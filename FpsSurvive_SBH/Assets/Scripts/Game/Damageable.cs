using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Game
{
    public class Damageable : MonoBehaviour
    {
		#region Variables
		private Health health;

		[SerializeField]
		private float damageRatio = 1f;
		[SerializeField]
		private float selfDamageRatio = 0.25f;
		#endregion

		private void Awake()
		{
			health = GetComponent<Health>();
			if(health == null)
			{
				health = GetComponentInParent<Health>();
			}
		}

		public void InflictDamage(float damage, bool isExplosion, GameObject damageSource)
		{
			if(health == null)
			{
				return;
			}

			var totalDamage = damage;

			if(damageSource == health.gameObject)
			{
				totalDamage *= selfDamageRatio;
			}

			if(isExplosion == false)
			{
				totalDamage *= damageRatio;
			}

			health.TakeDamage(totalDamage, damageSource);
		}
	}
}