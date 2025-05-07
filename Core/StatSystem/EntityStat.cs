using System.Linq;
using GM.Core.StatSystem;
using UnityEngine;

namespace GM.Entities
{
    public class EntityStat : MonoBehaviour, IEntityComponent
    {
        [SerializeField] protected StatOverride[] _statOverrides;

        protected StatSO[] _stats;
        protected Entity _entity;

        public virtual void Initialize(Entity entity)
        {
            _entity = entity;
            _stats = _statOverrides.Select(x => x.CreateStat()).ToArray();
        }

        public StatSO GetStat(StatSO stat)
        {
            StatSO findStat = _stats.FirstOrDefault(x => x.statName == stat.statName);
            Debug.Assert(findStat != null, $"stat is null : {stat.statName}");
            return findStat;
        }

        public bool TryGetStat(StatSO stat, out StatSO outStat)
        {
            outStat = _stats.FirstOrDefault(x => x.statName == stat.statName);
            return outStat != null;
        }

        public void SetBaseValue(StatSO stat, float value)
            => GetStat(stat).BaseValue = value;

        public float GetBaseValue(StatSO stat)
            => GetStat(stat).BaseValue;

        public float IncreaseBaseValue(StatSO stat, float value)
            => GetStat(stat).BaseValue += value;

        public void AddModifier(StatSO stat, object key, float value)
            => GetStat(stat).AddModifier(key, value);

        public void RemoveModifier(StatSO stat, object key)
            => GetStat(stat).RemoveModifier(key);

        public void ClearAllModifiers()
        {
            foreach (StatSO stat in _stats)
            {
                stat.ClearModifier();
            }
        }
    }
}
