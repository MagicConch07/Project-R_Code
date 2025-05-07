using System;
using System.Collections.Generic;
using GM.Data;
using GM.Entities;
using GM.Maps;
using UnityEngine;

namespace GM.Staffs
{
    public class StaffController : Entity, IPoolable
    {
        [SerializeField] private PoolTypeSO _poolType;

        [Header("Staff Element")]
        [SerializeField] private Waiter _waiter;
        [SerializeField] private Chef _chef;

        public StaffInfo Info => _staffInfo;
        public StaffType MyType => _type;
        public bool IsChange { get => _isChange; set => _isChange = value; }
        public PoolTypeSO PoolType => _poolType;
        public GameObject GameObject => gameObject;

        private bool _isChange;
        private StaffInfo _staffInfo;
        private StaffType _type = StaffType.Waiter;

        protected override void Awake()
        {
            _components = new Dictionary<Type, IEntityComponent>();
            AddComponentToDictionary(gameObject, _components, false);
            ComponentInitialize();
            AfterInitialize();

            SetStaff();
        }

        public void Initialize(StaffInfo info)
        {
            _staffInfo = info;
            _waiter.StaffInitialize();
            _chef.StaffInitialize();
        }

        private void Update()
        {
            // SyncTransform
            Transform targetTrm = GetStaff(_type).transform;
            GetStaff(_type, true).transform.SetPositionAndRotation(targetTrm.position, targetTrm.rotation);
        }

        public void SetStaff()
        {
            _waiter.gameObject.SetActive(false);
            _chef.gameObject.SetActive(false);
        }

        public Staff GetStaff(StaffType type, bool inverse = false)
        {
            if (inverse)
            {
                type -= 1;
            }
            return type == StaffType.Waiter ? _waiter : _chef;
        }

        public void SetStaffRestRoom(StaffRestPart restPart)
        {
            _waiter.SetStaffRestRoom(restPart);
            _chef.SetStaffRestRoom(restPart);

            _type = restPart.StaffType;
        }

        public void StartWork()
        {
            GetStaff(_type).gameObject.SetActive(true);
            GetStaff(_type).IdleState();
        }

        public void LeaveWork()
        {
            GetStaff(_type).LeaveWork();
        }

        public void SetUpPool(Pool pool)
        {
        }

        public void ResetItem()
        {
        }
    }
}
