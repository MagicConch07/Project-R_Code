using GM.GameEventSystem;
using GM.Maps;
using UnityEngine;

namespace GM.Players.Pickers.Maps
{
    public class MapSelector : MapEditor, IMapEditable
    {
        [SerializeField] private GameEventChannelSO _mapUIEventChannel;

        public override MapEditType EditType => MapEditType.Select;
        private MapObjectPart _selectMapPart;

        public void EditMap(Vector2Int position)
        {
            if (_map.TryGetMapPart(position, out MapObjectPart mapPart))
            {
                if (_selectMapPart == mapPart) return;

                _selectMapPart?.ShowInfo(false);

                mapPart.ShowInfo(true);
                _selectMapPart = mapPart;

                if (mapPart is StaffRestPart staffRestPart)
                {
                    MapUIEvents.SelectRestPart.restPart = staffRestPart;
                    _mapUIEventChannel.RaiseEvent(MapUIEvents.SelectRestPart);
                }

                MapEvents.SelectMapObjectPart.mapObjectPart = mapPart;
            }
            else
            {
                if (_selectMapPart == null) return;

                _selectMapPart.ShowInfo(false);
                _selectMapPart = null;
                MapEvents.SelectMapObjectPart.mapObjectPart = mapPart;
            }

            _mapUIEventChannel.RaiseEvent(MapEvents.SelectMapObjectPart);
        }
    }
}