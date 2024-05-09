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
		private AnimationCurve damageOverDistance;  //�Ÿ���� ������ ����
		#endregion

		public void InflictAreaDamage(float damage, Vector3 center, LayerMask layer, QueryTriggerInteraction interaction, GameObject damageSource)
		{
			Dictionary<Health, Damageable> uniqueDamageToHealth = new Dictionary<Health, Damageable>();
			
			//������ ���� ����
			Collider[] colliders = Physics.OverlapSphere(center, damageArea, layer, interaction);
			//������ ���� �ܵ�ȭ(�ټ��� �ǰ� ��������� �ִ� ����� �������� �������� �޴°� ����)
			foreach(Collider collider in colliders)
			{
				Debug.Log($"collider : {collider}");
				Damageable damageable = collider.GetComponent<Damageable>();
				if(damageable)
				{
					Debug.Log(damageable);
					Health health = damageable.GetComponentInParent<Health>();
					if(health != null && uniqueDamageToHealth.ContainsKey(health) == false)
					{
						uniqueDamageToHealth.Add(health, damageable);
					}
				}
			}

			//������ �߻�
			foreach(var uniqueDamageable in uniqueDamageToHealth.Values)
			{
				float distance = Vector3.Distance(center, uniqueDamageable.transform.position);
				float curvedDamage = damage * damageOverDistance.Evaluate(1 - (distance / damageArea));
				uniqueDamageable.InflictDamage(curvedDamage, true, damageSource);
				Debug.Log($"Damage To {uniqueDamageable}");
			}
		}
	}
}