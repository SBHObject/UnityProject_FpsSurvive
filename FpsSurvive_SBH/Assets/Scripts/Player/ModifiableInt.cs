using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    //�Ӽ��� ���
    [System.Serializable]
    public class ModifiableInt
    {
        #region Variables
        [NonSerialized]
        private int baseValue;

        [SerializeField]
        private int modifiedValue;

        public int BaseValue
        {
            get { return baseValue; }
            set
            {
                baseValue = value;

            }
        }

        public int ModifiedValue
        {
            get { return modifiedValue; }
            set { modifiedValue = value; }
        }

        private event Action<ModifiableInt> OnModified;

        //�Ӽ��� ����Ʈ
        private List<int> modifiers = new List<int>();
        #endregion

        //������
        public ModifiableInt(Action<ModifiableInt> method = null)
        {
            modifiedValue = baseValue;
            RegisterModEvent(method);
        }

        //�Ӽ��� ������Ʈ�� ȣ��Ǵ� �Լ� �߰�
        public void RegisterModEvent(Action<ModifiableInt> method)
        {
            if(method != null)
            {
                OnModified += method;
            }
        }

        //����
        public void RemoveModEvent(Action<ModifiableInt> method)
        {
            if(method !=null)
            {
                OnModified -= method;
            }
        }

        //���氪 ����
        private void UpdateModifiedValue()
        {
            int valueToAdd = 0;
            foreach (int modifier in modifiers)
            {
                valueToAdd += modifier;
            }

            //������
            ModifiedValue = baseValue + valueToAdd;
            OnModified?.Invoke(this);
        }

        public void AddModifier(int modifier)
        {
            modifiers.Add(modifier);
            UpdateModifiedValue();
        }

        public void RemoveModifier(int modifier)
        {
            modifiers.Remove(modifier);
            UpdateModifiedValue();
        }
    }
}