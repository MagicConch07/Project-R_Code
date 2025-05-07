using UnityEngine;

namespace GM.Players.Pickers.Maps
{
    public class MapRotator : MapEditor, IMapEditable
    {
        public override MapEditType EditType => MapEditType.Rotate;

        public void EditMap(Vector2Int position)
        {
            if (_mapPicker.CurrentMapObject == null) return;

            _mapPicker.CurrentMapObject.RotateObject();
        }
    }
}
