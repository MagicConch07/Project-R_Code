using GM.Core.StatSystem;
using GM.Entities;
using GM.Staffs;
using GM.Utility;
using Unity.Behavior;
using UnityEngine;

namespace GM.BT
{
    public class BTStat : MonoBehaviour, IEntityComponent, IAfterInitable
    {
        [SerializeField] protected BehaviorGraphAgent _btAgent;

        [SerializeField] private SerializedDictionary<string, StatSO> _btStatMappingDictionary = new SerializedDictionary<string, StatSO>();

        private Staff _staff;
        private EntityStat _stat;

        public void Initialize(Entity entity)
        {
            _staff = entity as Staff;
        }

        public void AfterInit()
        {
            _stat = _staff.MyStaffHandler.GetCompo<EntityStat>();

            foreach (var mappingStat in _btStatMappingDictionary)
            {
                _btAgent.SetVariableValue<float>(mappingStat.Key, _stat.GetStat(mappingStat.Value).Value);
                _stat.GetStat(mappingStat.Value).OnValueChange += (StatSO stat, float current, float prev) =>
                {
                    _btAgent.SetVariableValue<float>(mappingStat.Key, current);
                };
            }
        }

        private void OnDestroy()
        {
            foreach (var mappingStat in _btStatMappingDictionary)
            {
                _stat.GetStat(mappingStat.Value).OnValueChange = null;
            }
        }
    }
}
