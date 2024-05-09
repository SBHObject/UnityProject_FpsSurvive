using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FpsSurvive.Player
{
    public class Health : MonoBehaviour
    {
		#region Variables
		[SerializeField]
		private float maxHealth = 100;
		public float MaxHealth
		{
			get { return maxHealth; }
			private set { maxHealth = value; }
		}

		public float CurrentHealth { get; private set; }

		private bool isDeath = false;

		public UnityAction<float, GameObject> OnDamaged;    //데미지를 받을때 호출되는 이벤트함수<받은데미지, 데미지를준 객체>
		public UnityAction OnDie;                           //사망시
		public UnityAction<float> OnHeal;                   //회복시<받은 회복량>

		#endregion

		private void Start()
		{
			CurrentHealth = maxHealth;
		}

		public void TakeDamage(float damage, GameObject damageSource)
		{
			//실제 입는 데미지 량 구하기
			//계산 전 데미지 량 저장
			float beforeHealth = CurrentHealth;
			CurrentHealth -= damage;
			CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);   //현재체력 0 ~ 최대체력으로 제한

			//실제 입는 데미지량 구하기
			float realDamageAmount = beforeHealth - CurrentHealth;
			if (realDamageAmount > 0f)
			{
				//데미지 효과 규현(Ui, SFX 등등)
				OnDamaged?.Invoke(damage, damageSource);
			}

			Debug.Log($"Get Damage : {realDamageAmount}");
			HandleDeath();
		}

		public void TakeHeal(float healAmount)
		{
			if(CurrentHealth >= maxHealth)
			{
				return;
			}

			CurrentHealth += healAmount;
			CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

			OnHeal?.Invoke(healAmount);
		}

		//캐릭터 죽음 처리
		private void HandleDeath()
		{
			//사망 중복처리 방지
			if (isDeath == true)
				return;

			if (CurrentHealth <= 0f)
			{
				isDeath = true; //사망처리

				//사망효과 처리
				OnDie?.Invoke();

				Destroy(gameObject, 2f);
			}
		}

		public float GetHealthRatio()
		{
			return CurrentHealth / maxHealth;
		}
	}
}