using GM.Maps;
using UnityEngine;

namespace GM.Players.Pickers.Maps
{
    public class MapDeleter : MapEditor, IMapEditable, IMouseMapEditable
    {
        public override MapEditType EditType => MapEditType.Delete;

        public void EditMap(Vector2Int position)
        {
            _map.TryDeleteObject(position);
        }

        public void EditMapWithMouse(Vector2Int position)
        {
            _map.PreviewObject(position, MapPreviewType.Delete);
        }
    }
}
