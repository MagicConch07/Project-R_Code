using GM.Maps;
using GM.Players.Pickers.Maps;

namespace GM.GameEventSystem
{
    public static class MapEvents
    {
        public static readonly MapEditTypeChange MapEditTypeChange = new();
        public static readonly SelectMapObjectPart SelectMapObjectPart = new();
    }

    public class MapEditTypeChange : GameEvent
    {
        public MapEditType editType;
    }

    public class SelectMapObjectPart : GameEvent
    {
        public MapObjectPart mapObjectPart;
    }
}
