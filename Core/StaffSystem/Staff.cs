using GM.Data;
using GM.Entities;
using GM.InteractableEntities;
using GM.Maps;
using Unity.Behavior;
using UnityEngine;

namespace GM.Staffs
{
    public enum StaffType
    {
        Waiter,
        Chef
    }

    public abstract class Staff : Unit
    {
        public StaffType MyStaffType => _myStaffType;
        [SerializeField] private StaffType _myStaffType;

        public Transform FoodHandTrm => _foodHandTrm;
        [SerializeField] protected Transform _foodHandTrm;

        public StaffController MyStaffHandler => _staffHandler;
        [SerializeField] protected StaffController _staffHandler;

        public bool IsWorking => _isWorking;
        protected bool _isWorking = false;

        public StaffLevel Level => _level;
        protected StaffLevel _level;

        public ref OrderData CurrentData => ref _currentData;
        protected OrderData _currentData;

        protected StaffRestPart _myRestRoom;

        public bool IsChange { get => _isChange; set => _isChange = value; }
        private bool _isChange;

        protected BehaviorGraphAgent _myBTAgent;
        protected InteractableEntity _targetTable;

        protected override void Awake()
        {
            base.Awake();
            InitializedBT();
        }

        protected virtual void InitializedBT()
        {
            _myBTAgent = GetComponent<BehaviorGraphAgent>();
        }

        public abstract Transform GetTarget(Enums.InteractableEntityType type);
        public abstract void IdleState();
        public abstract void LeaveWork();

        public virtual void FinishWork()
        {
            _isWorking = false;
        }

        public void StaffInitialize()
        {
            _level = _staffHandler.GetCompo<StaffLevel>();
        }

        public void SetStaffRestRoom(StaffRestPart restRoom)
        {
            if (_myRestRoom != null)
            {
                _myRestRoom.SetStaffVisual(false);
            }

            _myRestRoom = restRoom;
        }

        public BlackboardVariable<T> GetVariable<T>(string variableName)
        {
            if (_myBTAgent.GetVariable(variableName, out BlackboardVariable<T> variable))
            {
                return variable;
            }
            return null;
        }

        public void SetVariable<T>(string variableName, T value)
        {
            BlackboardVariable<T> variable = GetVariable<T>(variableName);
            Debug.Assert(variable != null, $"Variable {variableName} not found");
            variable.Value = value;
        }

        public void StaffChangeEvent()
        {
            _staffHandler.ChangeProcess(_myStaffType);
            _isChange = false;
        }

        public void StaffEndChnage()
        {
            _staffHandler.IsChange = false;
        }

        public void SetTable(InteractableEntity table)
        {
            if (table == null || table.IsShared == true) return;

            _targetTable = table;
            _targetTable.InUse = true;
        }

        public void EndTable()
        {
            if (_targetTable == null) return;

            _targetTable.InUse = false;
        }

        public void EndWork()
        {
            gameObject.SetActive(false);
        }
    }
}
