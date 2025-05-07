using GM.Maps;
using UnityEngine;

namespace GM.Players.Pickers.Maps
{
    public class MapRelocator : MapEditor, IMapEditable, IMouseMapEditable, IMapEditAssistable
    {
        private bool _isRelocate = false;

        public override MapEditType EditType => MapEditType.Relocator;

        public override void StartEdit()
        {
            _isRelocate = false;
        }

        public override void EndEdit()
        {
            EditMapAssist();
        }

        protected override void HandleSetMapObjectInfo(ObjectInfoSO mapObjectInfo)
        {
            base.HandleSetMapObjectInfo(mapObjectInfo);

            if (mapObjectInfo != null)
            {
                _isRelocate = true;
            }
            else
            {
                _isRelocate = false;
            }
        }

        public void EditMap(Vector2Int position)
        {
            if (_isRelocate)
            {
                if (_map.TryRelocateObject(position, picker: _mapPicker, isRelocate: true))
                {
                    _isRelocate = false;
                }
            }
            else
            {
                _map.TryRelocateObject(position, picker: _mapPicker);
            }
        }

        public void EditMapWithMouse(Vector2Int position)
        {
            if (_isRelocate)
            {
                _map.DeletePreviewObject(position, _mapPicker.CurrentMapObject);
            }
            else
            {
                _map.PreviewObject(position, MapPreviewType.Relocate);
            }
        }

        public void EditMapAssist()
        {
            if (_mapPicker.CurrentMapObject == null || _isRelocate == false) return;

            _map.RollbackRelocateObject();
            _isRelocate = false;
        }
    }
}
