using Unity.Behavior;

namespace GM
{
    [BlackboardEnum]
    public enum WaiterState
    {
        IDLE,
        ORDER,
        COUNT,
        SERVING,
        FoodTrash,
        LeaveWork,
        Change
    }
}
