using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    //속성값 계산
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

        //속성값 리스트
        private List<int> modifiers = new List<int>();
        #endregion

        //생성자
        public ModifiableInt(Action<ModifiableInt> method = null)
        {
            modifiedValue = baseValue;
            RegisterModEvent(method);
        }

        //속성값 업데이트시 호출되는 함수 추가
        public void RegisterModEvent(Action<ModifiableInt> method)
        {
            if(method != null)
            {
                OnModified += method;
            }
        }

        //제거
        public void RemoveModEvent(Action<ModifiableInt> method)
        {
            if(method !=null)
            {
                OnModified -= method;
            }
        }

        //변경값 적용
        private void UpdateModifiedValue()
        {
            int valueToAdd = 0;
            foreach (int modifier in modifiers)
            {
                valueToAdd += modifier;
            }

            //최종값
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