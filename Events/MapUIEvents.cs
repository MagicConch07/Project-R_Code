using GM.Maps;

namespace GM.GameEventSystem
{
    public static class MapUIEvents
    {
        public static readonly SelectRestPart SelectRestPart = new();
    }

    public class SelectRestPart : GameEvent
    {
        public StaffRestPart restPart;
    }
}
