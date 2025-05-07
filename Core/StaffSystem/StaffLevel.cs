using System;
using GM.Core.StatSystem;
using GM.Entities;
using GM.GameEventSystem;
using GM.Managers;
using UnityEngine;

namespace GM.Staffs
{
    [Serializable]
    public class StatLevelOverride
    {
        [SerializeField] private StatSO _stat;
        [SerializeField] private float _multipleValue;
    }

    public class StaffLevel : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private GameEventChannelSO _levelUpEventChannel;
        [SerializeField] private StatSO _talentStat;
        [SerializeField] private StatLevelOverride[] _statLevelOverride;

        private StaffController _staffHandler;
        private EntityStat _stat;

        public uint Quality => _quality;
        private uint _quality;

        public uint Level => _level;
        private uint _level;

        public uint NeedLevelUpMoney => _needLevelUpMoney;
        private uint _needLevelUpMoney;

        public void Initialize(Entity entity)
        {
            _staffHandler = entity as StaffController;
            _stat = _staffHandler.GetCompo<EntityStat>();
            LevelInitialize();
        }

        private void LevelInitialize()
        {
            _level = 1;
            SetNeedLevelUpMoney();
        }

        public void SetQuality(uint quality)
        {
            _quality = quality;
        }

        public void LevelUp()
        {
            if (!ManagerHub.Instance.GetManager<DataManager>().SubtractMoney(_needLevelUpMoney))
                return;

            _level++;
            SetNeedLevelUpMoney();

            UnitUIEvents.LevelUpUIEvent.level = _level;
            UnitUIEvents.LevelUpUIEvent.needLevelUpMoney = _needLevelUpMoney;
            _levelUpEventChannel.RaiseEvent(UnitUIEvents.LevelUpUIEvent);
        }

        private void SetNeedLevelUpMoney()
        {
            _needLevelUpMoney = CalculateFibonacci(_level) * (_quality + 2);
        }

        private uint CalculateFibonacci(uint n)
        {
            if (n <= 1) return n;
            uint a = 0;
            uint b = 1;
            for (uint i = 2; i <= n; i++)
            {
                uint temp = a;
                a = b;
                b = temp + b;
            }
            return b;
        }
    }
}
