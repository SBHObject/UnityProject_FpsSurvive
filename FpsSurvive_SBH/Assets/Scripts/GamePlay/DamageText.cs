using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FpsSurvive.GamePlay
{
    public class DamageText : MonoBehaviour
    {
        #region
        private Camera mainCamera;
        public GameObject textObject;

        public TextMeshProUGUI damageText;

        public int Damage { get; set; }
        //�� ������Ʈ�� ���� �ö� �ӵ�
        private float upSpeed = 0.01f;
        
        //������Ʈ ���� �ð�
        private float lifeTime = 2f;
        #endregion

        private void Start()
        {
            mainCamera = Camera.main;

            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            damageText.text = Damage.ToString();
            textObject.transform.LookAt(mainCamera.transform.position);

            transform.Translate(Vector3.up * upSpeed);
        }
    }
}