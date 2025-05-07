using GM.Data;
using GM.Entities;

namespace GM.GameEventSystem
{
    public enum DescriptionUIType
    {
        Unit,
        Table,
    }

    public static class UnitUIEvents
    {
        public static readonly UnitDescriptionUIEvent UnitDescriptionUIEvent = new();
        public static readonly UIDescriptionEvent UIDescriptionEvent = new();
        public static readonly PriorityUIEvent PriorityUIEvent = new();
        public static readonly LevelUpUIEvent LevelUpUIEvent = new();
    }

    public class UnitDescriptionUIEvent : GameEvent
    {
        public DescriptionUIType type;
        public Unit unit;
        public bool isActive;
    }

    public class UIDescriptionEvent : GameEvent
    {
        public bool UIState;
    }

    public class PriorityUIEvent : GameEvent
    {
        public OrderType type;
        public bool isDrag;
    }

    public class LevelUpUIEvent : GameEvent
    {
        public uint level;
        public uint needLevelUpMoney;
    }
}