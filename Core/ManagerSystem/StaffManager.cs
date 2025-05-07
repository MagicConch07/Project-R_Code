using GM.Data;
using GM.GameEventSystem;
using GM.Staffs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GM.Managers
{
    public class StaffManager : MonoBehaviour, IManagerable, IManagerUpdateable
    {
        public int StaffCount => _staffControllerList.Count;

        [SerializeField] private GameEventChannelSO _gameCycleChannel;
        [SerializeField] private Transform _entranceTransform;

        private List<StaffController> _staffControllerList;
        private LinkedList<OrderData> _orderList;
        private Queue<OrderData> _recipeQueue;
        private int _currentCounterOrderCount = 0;
        private int _currentStaffCount = 0;

        private void Awake()
        {
            _gameCycleChannel.AddListener<RestourantCycleEvent>(HandleRestourantCycleEvent);
        }

        public void Initialized()
        {
            _staffControllerList = new List<StaffController>();
            _orderList = new LinkedList<OrderData>();
            _recipeQueue = new Queue<OrderData>();
        }

        public void Clear()
        {
            _gameCycleChannel.RemoveListener<RestourantCycleEvent>(HandleRestourantCycleEvent);

            _staffControllerList.Clear();
            _orderList.Clear();
            _recipeQueue.Clear();
        }

        public void DecreaseStaffCountAndCheckEmpty()
        {
            --_currentStaffCount;
            if (_currentStaffCount <= 0)
            {
                _gameCycleChannel.RaiseEvent(GameCycleEvents.AllStaffOut);
            }
        }

        public List<StaffController> GetStaffs()
        {
            return _staffControllerList.Select(staffController => staffController).ToList();
        }

        public void AddStaff(StaffController staffController)
        {
            _staffControllerList.Add(staffController);
        }

        public void RemoveStaff(StaffController staffController)
        {
            _staffControllerList.Remove(staffController);
        }

        public void Update()
        {
            ChefGiveWork();
            WaiterGiveWork();
        }

        /// <summary>
        /// Add Data for WaiterManager Order
        /// </summary>
        /// <param name="data">Order data</param>
        public void AddOrderData(OrderData data)
        {
            Debug.Assert(data.type != OrderType.Null, "OrderData Type is Null");

            if (data.type == OrderType.Cook)
            {
                _recipeQueue.Enqueue(data);
                return;
            }
            else if (data.type == OrderType.Count)
            {
                ++_currentCounterOrderCount;
            }

            _orderList.AddLast(data);
        }

        private void ChefGiveWork()
        {
            if (_recipeQueue.Count <= 0) return;

            Chef chef = GetWorkStaff(StaffType.Chef) as Chef;
            chef?.StartWork(ChefState.COOK, _recipeQueue.Dequeue());
        }

        private void WaiterGiveWork()
        {
            if (_orderList.Count <= 0) return;

            OrderData data;
            bool isContinuous = false;

            // Continuous calculation processing
            Waiter waiter = null;
            foreach (var staff in GetStaffs())
            {
                var counterWaiter = staff.GetStaff(StaffType.Waiter) as Waiter;
                if (counterWaiter.beforeWaiterState == WaiterState.COUNT)
                {
                    isContinuous = true;

                    if (!counterWaiter.IsWorking)
                    {
                        waiter = counterWaiter;
                        break;
                    }
                }
            }

            if (waiter != null && _currentCounterOrderCount >= 1)
            {
                data = _orderList.FirstOrDefault(x => x.type == OrderType.Count);
                waiter.StartWork(WaiterState.COUNT, data);
                --_currentCounterOrderCount;
                _orderList.Remove(data);
                return;
            }

            waiter = GetWorkStaff(StaffType.Waiter) as Waiter;

            if (waiter == null) return;

            data = !isContinuous ? _orderList.First() : _orderList.FirstOrDefault(x => x.type != OrderType.Count);

            if (data != default)
            {
                switch (data.type)
                {
                    case OrderType.Order:
                        waiter.StartWork(WaiterState.ORDER, data);
                        break;
                    case OrderType.Count:
                        waiter.StartWork(WaiterState.COUNT, data);
                        --_currentCounterOrderCount;
                        break;
                    case OrderType.Serving:
                        waiter.StartWork(WaiterState.SERVING, data);
                        break;
                }
                _orderList.Remove(data);
            }
        }

        private Staff GetWorkStaff(StaffType type)
        {
            return GetStaffs().Where(x => x.MyType == type && !x.IsChange)
            .Select(x => x.GetStaff(type))
            .Where(x => x != null && !x.IsWorking)
            .FirstOrDefault();
        }

        private IEnumerator GenerateStaff()
        {
            foreach (StaffController staffHandler in GetStaffs())
            {
                Staff staff = staffHandler.GetStaff(staffHandler.MyType);
                staff.transform.position = _entranceTransform.position;
                staff.transform.rotation = _entranceTransform.rotation;
                ++_currentStaffCount;
                staffHandler.StartWork();

                yield return new WaitForSeconds(0.7f);
            }

            _gameCycleChannel.RaiseEvent(GameCycleEvents.ReadyToRestourant);
            yield return null;
        }

        private void HandleRestourantCycleEvent(RestourantCycleEvent evt)
        {
            if (evt.open)
            {
                // GenerateStaff
                StartCoroutine(GenerateStaff());
            }
            else
            {
                // EndStaff
                foreach (StaffController staffHandler in GetStaffs())
                {
                    staffHandler.LeaveWork();
                }
            }
        }
    }
}
