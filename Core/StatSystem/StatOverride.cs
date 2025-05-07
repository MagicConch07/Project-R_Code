using System;
using UnityEngine;

namespace GM.Core.StatSystem
{
    [Serializable]
    public class StatOverride
    {
        [SerializeField] private StatSO _stat;
        [SerializeField] private bool _isUseOverride;
        [SerializeField] private float _overrideValue;

        public StatOverride(StatSO stat)
        {
            _stat = stat;
        }

        public StatSO CreateStat()
        {
            StatSO newStat = _stat.Clone() as StatSO;

            if (_isUseOverride)
                newStat.BaseValue = _overrideValue;

            return newStat;
        }
    }
}