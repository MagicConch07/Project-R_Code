using UnityEngine;

namespace GM.Players.Pickers.Maps
{
    public class MapCreator : MapEditor, IMapEditable, IMouseMapEditable
    {
        public override MapEditType EditType => MapEditType.Create;

        public void EditMap(Vector2Int position)
        {
            _map.TrySetMapObject(_mapPicker.CurrentMapObject, position, _mapPicker);
        }

        public void EditMapWithMouse(Vector2Int position)
        {
            _map.TrySetMapObject(_mapPicker.CurrentMapObject, position, _mapPicker, true);
        }
    }
}
