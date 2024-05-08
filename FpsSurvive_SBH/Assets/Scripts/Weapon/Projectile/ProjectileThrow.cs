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

		//���� (������ ��� �۵�)
		[SerializeField]
		private float detectRange = 0.5f;   //�ݶ��̴� ���� ����

		public Transform center;

		[SerializeField]
		private float fuseLifeTime = -1;    //-1�ϰ��, �Ű��� ����(���� �۵� ���)
		[SerializeField]
		private float throwForce = 20f;
		
		public LayerMask hittableLayer = -1;    //��� �۵�
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