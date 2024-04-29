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
		public float CurrentHealth { get; private set; }

		private bool isDeath = false;

		public UnityAction<float, GameObject> OnDamaged;    //�������� ������ ȣ��Ǵ� �̺�Ʈ�Լ�<����������, ���������� ��ü>
		public UnityAction OnDie;                           //�����
		public UnityAction<float> OnHeal;                   //ȸ����<���� ȸ����>

		#endregion

		private void Start()
		{
			CurrentHealth = maxHealth;
		}

		public void TakeDamage(float damage, GameObject damageSource)
		{
			//���� �Դ� ������ �� ���ϱ�
			//��� �� ������ �� ����
			float beforeHealth = CurrentHealth;
			CurrentHealth -= damage;
			CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);   //����ü�� 0 ~ �ִ�ü������ ����

			//���� �Դ� �������� ���ϱ�
			float realDamageAmount = beforeHealth - CurrentHealth;
			if (realDamageAmount > 0f)
			{
				//������ ȿ�� ����(Ui, SFX ���)
				OnDamaged?.Invoke(damage, damageSource);
			}

			HandleDeath();
		}

		//ĳ���� ���� ó��
		private void HandleDeath()
		{
			//��� �ߺ�ó�� ����
			if (isDeath == true)
				return;

			if (CurrentHealth <= 0f)
			{
				isDeath = true; //���ó��

				//���ȿ�� ó��
				OnDie?.Invoke();
			}
		}
	}
}