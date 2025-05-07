using GM.Data;
using GM.InteractableEntities;
using GM.Managers;
using GM.Maps;
using UnityEngine;

namespace GM.Staffs
{
    public class Waiter : Staff
    {
        public WaiterState beforeWaiterState = WaiterState.IDLE;

        [SerializeField] private WaiterStateChange _stateChangeEvent;

        protected override void InitializedBT()
        {
            base.InitializedBT();
            _stateChangeEvent = _stateChangeEvent.Clone() as WaiterStateChange;
            SetVariable("StateChange", _stateChangeEvent);
        }

        public void StartWork(WaiterState workType, OrderData data)
        {
            beforeWaiterState = workType;
            _currentData = data;
            _isWorking = true;
            _stateChangeEvent.SendEventMessage(workType);
        }

        public override Transform GetTarget(Enums.InteractableEntityType type)
        {
            InteractableEntity moveTarget;

            if (type == Enums.InteractableEntityType.Rest)
            {
                return _myRestRoom.transform;
            }
            else if (type == Enums.InteractableEntityType.Order)
            {
                SetVariable("OrderCustomerTrm", _currentData.orderCustomer.transform);
                return ManagerHub.Instance.GetManager<RestourantManager>().GetTable(_currentData.orderTableID).GetWaiterStandTrm(transform);
            }
            else if (type == Enums.InteractableEntityType.FoodOut)
            {
                if (ManagerHub.Instance.GetManager<RestourantManager>().GetInteractableEntity(type, out moveTarget, this))
                {
                    FoodOut foodOut = moveTarget as FoodOut;
                    SetTable(foodOut);
                    SetVariable("FoodTrm", foodOut.FoodTrm);
                    SetVariable("TableFoodTrm", ManagerHub.Instance.GetManager<RestourantManager>().GetTable(_currentData.orderTableID).GetFoodPos(_currentData.orderCustomer));
                    return foodOut.ReceiverTransform;
                }
            }
            else if (type == Enums.InteractableEntityType.Counter)
            {
                if (ManagerHub.Instance.GetManager<RestourantManager>().GetInteractableEntity(type, out moveTarget, this))
                {
                    SingleCounterEntity counter = moveTarget as SingleCounterEntity;
                    SetTable(counter);
                    return counter.SenderTransform;
                }
            }
            else if (type == Enums.InteractableEntityType.FoodTrashContainer)
            {
                if (ManagerHub.Instance.GetManager<RestourantManager>().GetInteractableEntity(type, out moveTarget, this))
                {
                    FoodTrashContainer foodTrash = moveTarget as FoodTrashContainer;
                    SetTable(foodTrash);
                    SetVariable("FoodTrashTrm", foodTrash.FoodTrashTransform);
                    _isWorking = true;
                    return foodTrash.EntityTransform;
                }
            }
            else if (type == Enums.InteractableEntityType.Exit)
            {
                if (ManagerHub.Instance.GetManager<RestourantManager>().GetInteractableEntity(type, out moveTarget, this))
                {
                    ExitPos exit = moveTarget as ExitPos;
                    return exit.transform;
                }
            }
            else if (ManagerHub.Instance.GetManager<RestourantManager>().GetInteractableEntity(type, out moveTarget, this))
            {
                SingleTableEntity singleTableEntity = moveTarget as SingleTableEntity;
                SetTable(singleTableEntity);
                return singleTableEntity.EntityTransform;
            }

            return null;
        }

        public override void LeaveWork()
        {
            _stateChangeEvent.SendEventMessage(WaiterState.LeaveWork);
        }

        public override void IdleState()
        {
            _stateChangeEvent.SendEventMessage(WaiterState.IDLE);
        }
    }
}