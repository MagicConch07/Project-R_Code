using System;
using System.Collections.Generic;
using UnityEngine;

namespace GM.Core.StatSystem
{
    [CreateAssetMenu(fileName = "StatSO", menuName = "SO/StatSystem/Stat")]
    public class StatSO : ScriptableObject, ICloneable
    {
        public delegate void ValueChangeHandler(StatSO stat, float current, float prev);
        public ValueChangeHandler OnValueChange;

        public string statName;
        [TextArea] public string description;
        public Sprite icon;
        public string displayName;
        [SerializeField] private float _baseValue, _minValue, _maxValue;

        private Dictionary<object, float> _modifyValueByKeys = new Dictionary<object, float>();

        [field: SerializeField] public bool IsPercent { get; private set; }

        private float _modifiedValue = 0; //수정된 값을 캐싱

        public float MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public float MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        public float Value => Mathf.Clamp(_baseValue + _modifiedValue, MinValue, MaxValue);
        public bool IsMax => Mathf.Approximately(Value, MaxValue);
        public bool IsMin => Mathf.Approximately(Value, MinValue);

        public float BaseValue
        {
            get => _baseValue;
            set
            {
                float prevValue = Value;
                _baseValue = Mathf.Clamp(value, MinValue, MaxValue);
                TryInvokeValueChangeEvent(Value, prevValue);
            }
        }

        private void TryInvokeValueChangeEvent(float value, float prevValue)
        {
            if (Mathf.Approximately(value, prevValue) == false)
            {
                OnValueChange?.Invoke(this, value, prevValue);
            }
        }

        public void AddModifier(object key, float value)
        {
            if (_modifyValueByKeys.ContainsKey(key)) return;
            //만약 중첩을 만들꺼라면 다른방식으로 구현해야 한다.

            float prevValue = Value; //이전 값 저장

            _modifiedValue += value;
            _modifyValueByKeys.Add(key, value);

            TryInvokeValueChangeEvent(Value, prevValue);
        }

        public void RemoveModifier(object key)
        {
            if (_modifyValueByKeys.TryGetValue(key, out float value))
            {
                float prevValue = Value;
                _modifiedValue -= value;
                _modifyValueByKeys.Remove(key);

                TryInvokeValueChangeEvent(Value, prevValue);
            }
        }

        public void ClearModifier()
        {
            float prevValue = Value;
            _modifyValueByKeys.Clear();
            _modifiedValue = 0;
            TryInvokeValueChangeEvent(Value, prevValue);
        }

        public object Clone() => Instantiate(this);
    }
}
