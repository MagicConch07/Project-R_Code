using System;
using System.Collections.Generic;
using System.Linq;
using GM.Entities;
using GM.GameEventSystem;
using GM.Inputs;
using GM.Managers;
using GM.Maps;
using UnityEngine;

namespace GM.Players.Pickers.Maps
{
    public class MapPicker : Picker
    {
        [SerializeField] private GameEventChannelSO _mapEventChannel;

        public event Action<ObjectInfoSO> SetObjectInfoEvent;
        public event Action<MapEditType> OnChangeMapEditTypeEvent;

        public Vector3 HitPosition => _hit.point;
        public Vector2Int GridPosition
        {
            get
            {
                Vector3Int currentGridPos = _grid.WorldToCell(HitPosition);
                if (_lastGridPosition != currentGridPos)
                {
                    _cachedGridPosition = new Vector2Int(currentGridPos.x, currentGridPos.z);
                    _lastGridPosition = currentGridPos;
                }
                return _cachedGridPosition;
            }
        }

        public Vector3 GridToWorldPosition
        {
            get
            {
                Vector3Int gridPos = _grid.WorldToCell(HitPosition);
                return _grid.CellToWorld(gridPos);
            }
        }

        public MapObject CurrentMapObject => _currentMapObject;
        public MapEditType EditType => _currentEditType;

        [SerializeField] private Map _map;
        [SerializeField] private Grid _grid;
        [SerializeField] private Transform _editorsTrm;

        private Dictionary<MapEditType, MapEditor> _mapEditors;
        private MapObject _currentMapObject;
        private Vector2Int _cachedGridPosition;
        private Vector3Int _lastGridPosition;
        private MapEditType _currentEditType;
        private InputType _currentInputType;
        private MapEditInput _mapEditInput;
        private bool _isEditorActive = false;

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            InitializeMapEditors();
            InitializeAllEditors();
            SubscribeToInputEvents();
            EditorsActive(false);
        }

        private void OnDestroy()
        {
            UnsubscribeFromInputEvents();
            SetObjectInfoEvent = null;
        }

        private void Update()
        {
            HandlePick();
        }

        public void EndMapEdit()
        {
            EditorsActive(false);
            SetMapObjectInfo(null);
        }

        public void SetUIInteraction(bool isUIInteraction)
        {
            GetEditor<MapMover>(MapEditType.Move).CellIndicatorActive(!isUIInteraction);
            CurrentMapObject?.SetObjectActive(!isUIInteraction);
        }

        public void SetMapEditType(MapEditType editType)
        {
            _currentEditType = editType;
            _map.InitSelectObject();
            OnChangeMapEditTypeEvent?.Invoke(editType);

            // TODO : GameEvent 방식으로 OnChangeMapEdityTypeEvent 바꾸기
            //! 전부다

            MapEvents.MapEditTypeChange.editType = _currentEditType;
            _mapEventChannel.RaiseEvent(MapEvents.MapEditTypeChange);
        }

        public void SetMapObjectInfo(ObjectInfoSO mapObjectInfo, RotationDirection rotationType = RotationDirection.Null)
        {
            if (_currentInputType != InputType.MapEdit)
            {
                _currentMapObject?.gameObject.SetActive(false);
                return;
            }

            ReleaseCurrentMapObject();

            if (mapObjectInfo == null)
            {
                _currentMapObject = null;
                SetObjectInfoEvent?.Invoke(null);
                return;
            }

            GetMapObjectFromPool(mapObjectInfo.poolType);
            SetMapObjectTransfrom(mapObjectInfo.objectSize.y, rotationType);

            SetObjectInfoEvent?.Invoke(mapObjectInfo);
        }

        public void RotateMapObject()
        {
            GetEditor<IMapEditable>(MapEditType.Rotate)?.EditMap(GridPosition);
        }

        protected override void PickEntity()
        {
            if (_isEditorActive == false) return;
            GetEditor<IMouseMapEditable>(_currentEditType)?.EditMapWithMouse(GridPosition);
        }

        private void InitializeMapEditors()
        {
            _mapEditors = _editorsTrm.GetComponentsInChildren<MapEditor>()
                        .ToDictionary(editor => editor.EditType);
        }

        private void InitializeAllEditors()
        {
            foreach (var editor in _mapEditors.Values)
            {
                editor.Initialize(this, _map);
            }
        }

        private void SubscribeToInputEvents()
        {
            var input = _player.Input;
            var mapEditInput = input.GetInput<MapEditInput>();

            _mapEditInput = mapEditInput;

            input.OnInputTypeChangeEvent += HandleInputTypeChange;
            _mapEditInput.OnMapEditLeftClickEvent += HandleMapLeftClick;
            _mapEditInput.OnMapDragEvent += HandleMapDrag;
            _mapEditInput.OnRotateObjectEvent += HandleRotateObject;
            _mapEditInput.MapEditTypeChangeEvent += HandleMapTypeChange;
        }

        private void UnsubscribeFromInputEvents()
        {
            var input = _player.Input;

            input.OnInputTypeChangeEvent -= HandleInputTypeChange;
            _mapEditInput.OnMapEditLeftClickEvent -= HandleMapLeftClick;
            _mapEditInput.OnMapDragEvent -= HandleMapDrag;
            _mapEditInput.OnRotateObjectEvent -= HandleRotateObject;
            _mapEditInput.MapEditTypeChangeEvent -= HandleMapTypeChange;

            _mapEditInput = null;
        }

        private T GetEditor<T>(MapEditType type) where T : class
        {
            return _mapEditors.TryGetValue(type, out var editor) ? editor as T : null;
        }

        private void EditorsActive(bool activeValue)
        {
            foreach (MapEditor editor in _mapEditors.Values)
            {
                if (activeValue)
                {
                    editor.StartEdit();
                }
                else
                {
                    editor.EndEdit();
                }
            }

            _isEditorActive = activeValue;
            _editorsTrm.gameObject.SetActive(activeValue);
        }

        private void HandleInputTypeChange(InputType type)
        {
            _currentInputType = type;
            if (_currentInputType == InputType.MapEdit)
            {
                EditorsActive(true);
                _map.DeleteOutline();
            }
            else
            {
                EditorsActive(false);
            }

            HandlePick();
            _map.InitSelectObject();
        }

        private void HandleMapDrag(bool isDrag)
        {
            if (isDrag)
            {
                HandlePick();
            }
        }

        private void HandleMapLeftClick(bool isClick)
        {
            if (!isClick) return;

            GetEditor<IMapEditable>(_currentEditType)?.EditMap(GridPosition);
        }

        private void HandleRotateObject()
        {
            RotateMapObject();
        }

        private void HandleMapTypeChange()
        {
            int typeSize = (int)MapEditType.Delete + 1;
            int nextEditTypeIndex = ((int)_currentEditType) + 1;
            _currentEditType = (MapEditType)(nextEditTypeIndex % typeSize);

            _map.InitSelectObject();
            OnChangeMapEditTypeEvent?.Invoke(_currentEditType);

            // TODO : 위에랑 같이 로직 바꾸기
            MapEvents.MapEditTypeChange.editType = _currentEditType;
            _mapEventChannel.RaiseEvent(MapEvents.MapEditTypeChange);
        }


        private void ReleaseCurrentMapObject()
        {
            if (_currentMapObject != null)
            {
                ManagerHub.Instance.Pool.Push(_currentMapObject);
            }
        }

        private void GetMapObjectFromPool(PoolTypeSO poolType)
        {
            _currentMapObject = ManagerHub.Instance.Pool.Pop(poolType) as MapObject;
            _currentMapObject.SetCollider(false);
        }

        private void SetMapObjectTransfrom(float height, RotationDirection rotationType)
        {
            if (_currentMapObject == null) return;

            _currentMapObject.transform.position = new Vector3(
                    _currentMapObject.transform.position.x,
                    height,
                    _currentMapObject.transform.position.z);

            if (rotationType == RotationDirection.Null) return;
            _currentMapObject.IsObjectLock = false;
            _currentMapObject.RotateObject(rotationType);
        }
    }
}
