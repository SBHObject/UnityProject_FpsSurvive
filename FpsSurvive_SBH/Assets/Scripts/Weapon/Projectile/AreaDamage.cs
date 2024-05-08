using FpsSurvive.Game;
using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Weapon
{
    public class AreaDamage : MonoBehaviour
    {
		#region Variables
		[SerializeField]
		private float damageArea = 5f;
		[SerializeField]
		private AnimationCurve damageOverDistance;  //거리비례 데미지 감소
		#endregion

		public void InflictAreaDamage(float damage, Vector3 center, LayerMask layer, QueryTriggerInteraction interaction, GameObject damageSource)
		{
			Dictionary<Health, Damageable> uniqueDamageToHealth = new Dictionary<Health, Damageable>();
			
			//데미지 판정 범위
			Collider[] colliders = Physics.OverlapSphere(center, damageArea, layer, interaction);
			//데미지 판정 단독화(다수의 피격 대상지점이 있는 대상이 여러번의 데미지를 받는걸 방지)
			foreach(Collider collider in colliders)
			{
				Damageable damageable = collider.GetComponent<Damageable>();
				if(damageable)
				{
					Health health = damageable.GetComponentInParent<Health>();
					if(health != null && uniqueDamageToHealth.ContainsKey(health) == true)
					{
						uniqueDamageToHealth.Add(health, damageable);
					}
				}
			}

			//데미지 발생
			foreach(var uniqueDamageable in uniqueDamageToHealth.Values)
			{
				float distance = Vector3.Distance(center, uniqueDamageable.transform.position);
				float curvedDamage = damage * damageOverDistance.Evaluate(distance / damageArea);
				uniqueDamageable.InflictDamage(curvedDamage, true, damageSource);
			}
		}
	}
}