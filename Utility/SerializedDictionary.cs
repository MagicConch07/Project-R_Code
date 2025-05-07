using System;
using System.Collections.Generic;
using UnityEngine;

namespace GM.Utility
{
    [Serializable]
    public class SerializedKeyAndValue<K, V>
        where V : class
    {
        public K Key;
        public V Value;
    }

    [Serializable]
    public class SerializedDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
        where V : class
    {
        [SerializeField] private List<SerializedKeyAndValue<K, V>> _keyAndValueList;

        public void OnBeforeSerialize()
        {
            if (this.Count < _keyAndValueList.Count) return;

            _keyAndValueList.Clear();

            foreach (var keyAndValue in this)
            {
                _keyAndValueList.Add(new SerializedKeyAndValue<K, V>()
                {
                    Key = keyAndValue.Key,
                    Value = keyAndValue.Value
                });
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            foreach (var keyAndValue in _keyAndValueList)
            {
                if (!this.TryAdd(keyAndValue.Key, keyAndValue.Value))
                {
                    keyAndValue.Key = default;
                    keyAndValue.Value = null;
                }
            }
        }
    }
}
