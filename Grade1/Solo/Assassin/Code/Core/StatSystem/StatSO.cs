using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Core.StatSystem
{
    [CreateAssetMenu(fileName = "StatSO", menuName = "SO/StatSystem/Stat", order = 0)]
    public class StatSO : ScriptableObject, ICloneable
    {
        public delegate void ValueChangeHandler(StatSO stat, float current, float previous);
        public event ValueChangeHandler OnValueChange;

        public string statName;
        [TextArea]
        public string description;

        [SerializeField] private Sprite icon;
        [SerializeField] private string displayName;
        [SerializeField] private float baseValue, minValue, maxValue;
        
        private Dictionary<object, float> _modifyDictionary = new Dictionary<object, float>();
        
        [field: SerializeField] public bool IsPercent { get; private set; }
        
        private float _modifiedValue = 0;

        #region Property section
        
        public Sprite Icon => icon;
        public float MaxValue
        {
            get => maxValue;
            set => maxValue = value;
        }

        public float MinValue
        {
            get => minValue;
            set => minValue = value;
        }
        
        public float Value => Mathf.Clamp(baseValue  + _modifiedValue, MinValue, MaxValue);
        public bool IsMax => Mathf.Approximately(Value, MaxValue);
        public bool IsMin => Mathf.Approximately(Value, MinValue);

        public float BaseValue
        {
            get => baseValue;
            set
            {
                float prevValue = Value;
                baseValue = Mathf.Clamp(value, MinValue, MaxValue); //들어온 값을 clamp
                TryInvokeValueChangedEvent(Value, prevValue);
            }
        }
        
        #endregion

        public void AddModifier(object key, float value)
        {
            if (_modifyDictionary.ContainsKey(key)) return;
            float prevValue = Value; //이전 값을 꼭 기억해놨다가
            
            _modifiedValue += value;
            _modifyDictionary.Add(key, value);
            
            TryInvokeValueChangedEvent(Value, prevValue);
        }

        public void RemoveModifier(object key)
        {
            if (_modifyDictionary.TryGetValue(key, out float value))
            {
                float prevValue = Value;
                _modifiedValue -= value;
                _modifyDictionary.Remove(key);
                
                TryInvokeValueChangedEvent(Value, prevValue);
            }
        }

        public void ClearAllModifier()
        {
            float prevValue = Value;
            _modifyDictionary.Clear();
            _modifiedValue = 0;
            TryInvokeValueChangedEvent(Value, prevValue);
        }
        
        private void TryInvokeValueChangedEvent(float current, float prevValue)
        {
            //이진값과 일치하지 않으면 이벤트 인보크
            if(Mathf.Approximately(current, prevValue) == false)
                OnValueChange?.Invoke(this, current, prevValue);
        }
        
        public object Clone() => Instantiate(this);

    }
}