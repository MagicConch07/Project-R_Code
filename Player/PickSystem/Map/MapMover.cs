using GM.Maps;
using UnityEngine;

namespace GM.Players.Pickers.Maps
{
    public enum MoveEditType
    {
        Cell,
        MapObject,
    }

    public class MapMover : MapEditor
    {
        private const float Tile_HEIGHT = 0.5f;

        [SerializeField] private GameObject _cellIndicator;
        private MoveEditType _moveEditModeType;
        private bool _isLock = false;

        public override MapEditType EditType => MapEditType.Move;

        public override void StartEdit()
        {
            ChangeMoveEditMode(_moveEditModeType);
        }

        public override void EndEdit()
        {
            _cellIndicator?.gameObject.SetActive(false);
            _mapPicker.CurrentMapObject?.gameObject.SetActive(false);
        }

        public void CellIndicatorActive(bool isActive)
        {
            if (_cellIndicator != null)
            {
                _cellIndicator.SetActive(isActive);
            }
        }

        protected override void HandleSetMapObjectInfo(ObjectInfoSO mapObjectInfo)
        {
            _isLock = true;
            base.HandleSetMapObjectInfo(mapObjectInfo);

            if (mapObjectInfo == null)
            {
                ChangeMoveEditMode(MoveEditType.Cell);
            }
            else
            {
                ChangeMoveEditMode(MoveEditType.MapObject);
            }
        }

        protected override void HandleChangeMapEditType(MapEditType type)
        {
            _isLock = true;
            base.HandleChangeMapEditType(type);

            if (MapEditType.Create == type && _mapPicker.CurrentMapObject != null)
            {
                ChangeMoveEditMode(MoveEditType.MapObject);
            }
            else
            {
                ChangeMoveEditMode(MoveEditType.Cell);
            }
        }

        private void ChangeMoveEditMode(MoveEditType type)
        {
            _moveEditModeType = type;

            if (type == MoveEditType.Cell)
            {
                _cellIndicator.gameObject.SetActive(true);
                _mapPicker.CurrentMapObject?.gameObject.SetActive(false);
            }
            else if (type == MoveEditType.MapObject)
            {
                _mapPicker.CurrentMapObject?.gameObject.SetActive(true);
                _cellIndicator.gameObject.SetActive(false);
            }

            _isLock = false;
        }

        private void LateUpdate()
        {
            if (_isLock) return;

            Vector3 gridCellPos = _mapPicker.GridToWorldPosition;
            gridCellPos.y = Tile_HEIGHT;

            if (_moveEditModeType == MoveEditType.Cell)
            {
                _cellIndicator.transform.position = gridCellPos;
            }
            else if (_moveEditModeType == MoveEditType.MapObject && _mapPicker.CurrentMapObject != null)
            {
                var yPos = _mapPicker.EditType switch
                {
                    MapEditType.Relocator => gridCellPos.y + _mapObjectInfo.objectSize.y,
                    _ => gridCellPos.y
                };

                Vector3 objPos = new Vector3(gridCellPos.x + _mapObjectInfo.objectSize.x, yPos, gridCellPos.z + _mapObjectInfo.objectSize.z);
                _mapPicker.CurrentMapObject.transform.position = objPos;
            }
        }
    }
}
