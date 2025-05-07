using System.Collections.Generic;
using GM.CookWare;
using GM.InteractableEntities;
using GM.Managers;
using GM.Players.Pickers.Maps;
using Unity.AI.Navigation;
using UnityEngine;

namespace GM.Maps
{
    public enum MapPreviewType
    {
        Relocate,
        Delete,
        None
    }

    public readonly struct MapPlacementInfo
    {
        public readonly MapObject mapObject;
        public readonly Vector2Int position;
        public readonly Vector2Int size;
        public readonly Vector2Int direction;
        public readonly Vector2Int offset;

        public MapPlacementInfo(MapObject mapObject, Vector2Int position, Vector2Int size, Vector2Int direction, Vector2Int offset)
        {
            this.mapObject = mapObject;
            this.position = position;
            this.size = size;
            this.direction = direction;
            this.offset = offset;
        }
    }

    public class Map : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Transform _tilesTrm;
        [SerializeField] private Transform _mapObjectsTrm;
        [SerializeField] private ObjectInfoSO _wallInfo;
        [SerializeField] private Transform _navMeshTrm;

        private Dictionary<Vector2Int, MapArray> _mapDictionary;
        private NavMeshSurface _navMeshSurface;
        private List<Tile> _tempTileList;
        private MapObject _beforeSelectObject;
        private MapObject _relocateMapObject;

        private void Awake()
        {
            _mapDictionary = new Dictionary<Vector2Int, MapArray>();
            _tempTileList = new List<Tile>();

            _navMeshSurface = _navMeshTrm.GetComponent<NavMeshSurface>();
        }

        public void ActiveMapObjectVisual(bool isActive)
        {
            foreach (var mapObject in ManagerHub.Instance.GetManager<RestourantManager>().GetAllInteractableEntitiesList())
            {
                if (mapObject is IMapVisualer mapVisualObject)
                {
                    mapVisualObject.ActiveMapVisual(isActive);
                }
            }
        }

        public bool TryGetMapPart(Vector2Int position, out MapObjectPart mapPart)
        {
            mapPart = null;

            if (_mapDictionary.TryGetValue(position, out MapArray mapArray))
            {
                if (mapArray.MapPart != null)
                {
                    mapPart = mapArray.MapPart;
                    return true;
                }
            }

            return false;
        }

        public bool TrySetMapObject(MapObject mapObject, Vector2Int position, MapPicker picker, bool isCreatePreview = false)
        {
            if (mapObject == null) return false;
            mapObject.IsObjectLock = true;

            MapPlacementInfo mapPlacementInfo = CreateMapPlacementInfo(mapObject, position);

            if (!ValidateMapObjectInstallation(mapPlacementInfo))
            {
                return false;
            }

            mapObject.SetColor();

            // Installation
            if (!isCreatePreview)
            {
                if (!ManagerHub.Instance.GetManager<DataManager>().SubtractMoney(mapObject.Info.cost))
                {
                    // TODO : 돈 부족 로직 처리
                }

                var installMapObject = InstallMapObject(mapPlacementInfo);

                ManagerHub.Instance.GetManager<RestourantManager>().AddMapInteractable(installMapObject as InteractableEntity);
                picker.SetMapObjectInfo(null);
                ReBuildNavMesh();

                _mapObjectList.Add(mapObject);

                return true;
            }

            mapObject.IsObjectLock = false;
            return false;
        }

        public void InitSelectObject()
        {
            if (_beforeSelectObject != null)
            {
                _beforeSelectObject.SetColor();
            }
        }

        public bool TryDeleteObject(Vector2Int position)
        {
            if (!TryGetValidCell(position, out var mapArray))
            {
                return false;
            }

            MapObject currentObject = mapArray.MapPart.Owner;
            ManagerHub.Instance.GetManager<DataManager>().AddMoney(currentObject.Info.cost);
            ManagerHub.Instance.GetManager<RestourantManager>().RemoveMapInteractable(currentObject as InteractableEntity);
            currentObject.SetCollider(false);
            currentObject.DestroyObject();
            ReBuildNavMesh();

            _mapObjectList.Remove(currentObject);
            if (!_mapObjectList.Contains(currentObject) && currentObject.TryGetComponent(out CookingTable cookingTable))
            {
                ManagerHub.Instance.GetManager<RecipeManager>().CheckDelFabricOnRecipe(cookingTable.Info.type);
            }

            return true;
        }

        public bool TryRelocateObject(Vector2Int position, MapPicker picker = null, bool isRelocate = false)
        {
            if (!TryGetValidCell(position, out var mapArray, requireMapObject: isRelocate))
            {
                return false;
            }

            if (isRelocate)
            {
                var properties = CalculateMapObjectProperties(picker.CurrentMapObject);
                MapPlacementInfo mapPlacementInfo = new MapPlacementInfo
                (
                    _relocateMapObject,
                    position,
                    properties.size,
                    properties.direction,
                    properties.offset
                );

                if (!ValidateMapObjectInstallation(mapPlacementInfo))
                {
                    return false;
                }

                _relocateMapObject?.DestroyObject(false);
                _relocateMapObject?.SetObjectActive(true);

                Vector3 installObjectPos = CalculateInstallPosition(mapPlacementInfo.mapObject, mapPlacementInfo.position);
                _relocateMapObject.transform.localPosition = installObjectPos;
                _relocateMapObject.RotateObject(picker.CurrentMapObject.CurrentRotation);
                SetMapPartOfMapArray(mapPlacementInfo.mapObject);

                _relocateMapObject = null;

                ReBuildNavMesh();

                picker.SetMapObjectInfo(null);
                return true;
            }
            // Select Relocate Object
            else
            {
                _relocateMapObject = mapArray.MapPart.Owner;

                if (picker != null)
                {
                    picker.SetMapObjectInfo(_relocateMapObject.Info, mapArray.MapPart.Owner.CurrentRotation);
                }

                _relocateMapObject.SetTemporaryState(true);
                _relocateMapObject.SetCollider(false);
                _relocateMapObject.SetObjectActive(false);
                ReBuildNavMesh();
                return true;
            }
        }

        public void RollbackRelocateObject()
        {
            if (_relocateMapObject == null) return;
            InitSelectObject();
            _relocateMapObject.SetTemporaryState(false);
            _relocateMapObject.SetObjectActive(true);
            _relocateMapObject = null;
        }

        public void PreviewObject(Vector2Int position, MapPreviewType previewType)
        {
            InitSelectObject();

            if (!TryGetValidCell(position, out var mapArray))
            {
                return;
            }

            _beforeSelectObject = mapArray.MapPart.Owner;
            _beforeSelectObject.SetColor(previewType);
        }

        public void DeletePreviewObject(Vector2Int position, MapObject mapObject)
        {
            InitSelectObject();

            MapPlacementInfo mapPlacementInfo = CreateMapPlacementInfo(mapObject, position);

            if (!ValidateMapObjectInstallation(mapPlacementInfo))
            {
                _beforeSelectObject = mapObject;
                return;
            }
        }

        private MapPlacementInfo CreateMapPlacementInfo(MapObject mapObject, Vector2Int position)
        {
            var properties = CalculateMapObjectProperties(mapObject);
            return new MapPlacementInfo
            (
                mapObject,
                position,
                properties.size,
                properties.direction,
                properties.offset
            );
        }

        /// <summary>
        /// Calculates the properties of a MapObject based on its rotation and cell size.
        /// </summary>
        /// <param name="mapObject">The MapObject whose properties are being calculated.</param>
        /// <returns>
        /// A tuple containing:
        /// - size: The size of the MapObject in grid cells.
        /// - direction: The directional vector based on the MapObject's rotation.
        /// - offset: The adjusted offset for centering the object in the grid.
        /// </returns>
        private (Vector2Int size, Vector2Int direction, Vector2Int offset) CalculateMapObjectProperties(MapObject mapObject)
        {
            int objSizeX = mapObject.Info.cellSize.x;
            int objSizeY = mapObject.Info.cellSize.y;

            Vector2Int direction = CalculateMapObjectDirection(mapObject);

            if (Mathf.Abs(mapObject.GetNormalizedRotationY()) == 90)
            {
                int temp = objSizeX;
                objSizeX = objSizeY;
                objSizeY = temp;
            }

            Vector2Int offset = new Vector2Int(
                ConvertToCellCoordSize(0, ref objSizeX),
                ConvertToCellCoordSize(0, ref objSizeY));

            Vector2Int size = new Vector2Int(objSizeX, objSizeY);

            return (size, direction, offset);
        }

        private Vector2Int CalculateMapObjectDirection(MapObject mapObject)
        {
            Vector2Int direction = Vector2Int.one;
            direction.x = IsNegativeXDirection(mapObject.CurrentRotation) ? -1 : 1;
            direction.y = IsNegativeYDirection(mapObject.CurrentRotation) ? -1 : 1;
            return direction;
        }

        private bool IsNegativeXDirection(RotationDirection rotation) =>
            rotation == RotationDirection.Left || rotation == RotationDirection.Up;

        private bool IsNegativeYDirection(RotationDirection rotation) =>
            rotation == RotationDirection.Left || rotation == RotationDirection.Down;

        /// <summary>
        /// Converts a value to cell coordinate size for centering objects in a grid-based map
        /// </summary>
        /// <param name="value">The initial coordinate value</param>
        /// <param name="discriminantValue">The size value used to determine centering offset (reference parameter)</param>
        /// <returns>Adjusted coordinate value for cell positioning</returns>
        private int ConvertToCellCoordSize(int value, ref int discriminantValue)
        {
            // not even num
            if ((discriminantValue & 1) == 1)
            {
                value = discriminantValue / 2 * -1;
                discriminantValue = discriminantValue / 2 + 1;
            }

            return value;
        }

        /// <summary>
        /// Checks if a MapObject can be placed at the specified position on the map.
        /// </summary>
        /// <param name="mapObject">The MapObject to validate for placement.</param>
        /// <param name="position">The starting position to validate the placement.</param>
        /// <param name="size">The size of the MapObject in grid cells (width and height).</param>
        /// <param name="direction">The directional vector based on the MapObject's rotation.</param>
        /// <param name="offset">The starting offset for the MapObject's placement in the grid.</param>
        /// <returns>True if the MapObject can be placed at the specified position; otherwise, false.</returns>
        private bool ValidateMapObjectInstallation(MapPlacementInfo mapPlacementInfo)
        {
            for (int x = mapPlacementInfo.offset.x; x < mapPlacementInfo.size.x; ++x)
            {
                for (int y = mapPlacementInfo.offset.y; y < mapPlacementInfo.size.y; ++y)
                {
                    int posX = mapPlacementInfo.position.x + x * mapPlacementInfo.direction.x;
                    int posY = mapPlacementInfo.position.y + y * mapPlacementInfo.direction.y;
                    Vector2Int pos = new Vector2Int(posX, posY);

                    if (!TryGetValidCell(pos, out var cellData, requireMapObject: true))
                    {
                        SetInstallationFailure(mapPlacementInfo.mapObject);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool TryGetValidCell(Vector2Int position, out MapArray cellData, bool requireMapObject = false)
        {
            if (!_mapDictionary.TryGetValue(position, out cellData) ||
                cellData.tile == null ||
                (cellData.MapPart == requireMapObject && !cellData.IsTemp))
            {
                return false;
            }

            return true;
        }

        private void SetInstallationFailure(MapObject mapObject)
        {
            mapObject.SetColor(MapPreviewType.Delete);
            mapObject.IsObjectLock = false;
        }

        private MapObject InstallMapObject(MapPlacementInfo mapPlacementInfo)
        {
            Vector3 installObjectPos = CalculateInstallPosition(mapPlacementInfo.mapObject, mapPlacementInfo.position);

            MapObject installMapObject = ManagerHub.Instance.Pool.Pop(mapPlacementInfo.mapObject.Info.poolType) as MapObject;
            installMapObject.transform.SetParent(_mapObjectsTrm);
            installMapObject.transform.localPosition = installObjectPos;
            installMapObject.RotateObject(mapPlacementInfo.mapObject.CurrentRotation);

            SetMapPartOfMapArray(installMapObject);

            return installMapObject;
        }

        private void SetMapPartOfMapArray(MapObject mapObject)
        {
            for (int i = 0; i < mapObject.Parts.Length; ++i)
            {
                Vector3Int cellPos = _grid.WorldToCell(mapObject.Parts[i].transform.position);
                Vector2Int mapPos = new Vector2Int(cellPos.x, cellPos.z);

                if (_mapDictionary.TryGetValue(mapPos, out var mapArray))
                {
                    mapArray.SetMapPart(mapObject.Parts[i]);
                }
            }
        }


        private Vector3 CalculateInstallPosition(MapObject mapObject, Vector2Int position)
        {
            Vector3 installObjectPos = _grid.CellToWorld(new Vector3Int(position.x, 0, position.y));
            installObjectPos.x += mapObject.Info.objectSize.x;
            installObjectPos.y = mapObject.Info.objectSize.y;
            installObjectPos.z += mapObject.Info.objectSize.z;

            return installObjectPos;
        }

        #region TileMethod

        public void SetTemporaryTile(ObjectInfoSO mapObjectInfo, Vector2Int startPos, Vector2Int endPos)
        {
            // Initalize
            if (_tempTileList.Count > 0)
            {
                foreach (Tile tile in _tempTileList)
                {
                    tile.gameObject.SetActive(false);
                }
                _tempTileList.Clear();
            }

            List<Tile> tempTileList = new List<Tile>();

            // Calculate Distance
            short maxX = (short)Mathf.Max(startPos.x, endPos.x);
            short minX = (short)Mathf.Min(startPos.x, endPos.x);

            short maxY = (short)Mathf.Max(startPos.y, endPos.y);
            short minY = (short)Mathf.Min(startPos.y, endPos.y);

            for (short x = minX; x <= maxX; ++x)
            {
                for (short y = minY; y <= maxY; ++y)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (_mapDictionary.TryGetValue(pos, out var mapArray) &&
                        mapArray.tile != null)
                    {
                        if (mapArray.tile.IsFull) continue;

                        mapArray.tile.gameObject.SetActive(true);
                        mapArray.tile.ShowOutLine(true);
                        tempTileList.Add(mapArray.tile);
                        continue;
                    }

                    Vector3 mapObjPosition = _grid.CellToWorld(new Vector3Int(x, 0, y));
                    mapObjPosition.x += mapObjectInfo.objectSize.x;
                    mapObjPosition.y += mapObjectInfo.objectSize.y;
                    mapObjPosition.z += mapObjectInfo.objectSize.z;

                    Tile tile = ManagerHub.Instance.Pool.Pop(mapObjectInfo.poolType) as Tile;
                    tile.transform.position = mapObjPosition;
                    tile.transform.SetParent(_tilesTrm);

                    // even number
                    if (((x + y) & 1) == 0)
                    {
                        tile.SetColor(Color.white);
                    }
                    else
                    {
                        tile.SetColor(Color.black);
                    }
                    tile.SetCollider(false);
                    tile.ShowOutLine(true);
                    _mapDictionary.TryAdd(pos, new MapArray());
                    _mapDictionary.TryGetValue(pos, out var installMapArray);
                    installMapArray.tile = tile;
                    tempTileList.Add(tile);
                }
            }
            CreateWall(startPos, endPos);

            _tempTileList = tempTileList;
        }

        public void SetTileObject()
        {
            DeleteOutline();

            foreach (Tile tile in _tempTileList)
            {
                tile.IsFull = true;
                tile.SetCollider(true);
            }
            _tempTileList.Clear();
            _navMeshSurface.BuildNavMesh();
        }

        private void CreateWall(Vector2Int startPos, Vector2Int endPos)
        {
            short maxX = (short)Mathf.Max(startPos.x, endPos.x);
            short minX = (short)Mathf.Min(startPos.x, endPos.x);

            short maxY = (short)Mathf.Max(startPos.y, endPos.y);
            short minY = (short)Mathf.Min(startPos.y, endPos.y);

            for (short x = minX; x <= maxX; ++x)
            {
                for (short y = minY; y <= maxY; ++y)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    Vector2Int leftPos = new Vector2Int(pos.x - 1, pos.y);
                    Vector2Int upPos = new Vector2Int(pos.x, pos.y + 1);

                    if (!_mapDictionary.TryGetValue(leftPos, out var mapArrayLeft))
                    {
                        mapArrayLeft = new MapArray();
                        _mapDictionary.Add(leftPos, mapArrayLeft);
                    }

                    if (mapArrayLeft.tile == null)
                    {
                        Vector3 mapObjPosition = _grid.CellToWorld(new Vector3Int(x, 0, y));
                        mapObjPosition.x += _wallInfo.objectSize.x;
                        mapObjPosition.y += _wallInfo.objectSize.y;
                        mapObjPosition.z += _wallInfo.objectSize.z;

                        MapObject wall = ManagerHub.Instance.Pool.Pop(_wallInfo.poolType) as MapObject;
                        wall.transform.position = mapObjPosition;
                        wall.RotateObject(RotationDirection.Down);
                        wall.transform.SetParent(_tilesTrm);
                    }

                    if (!_mapDictionary.TryGetValue(upPos, out var mapArrayUp))
                    {
                        mapArrayUp = new MapArray();
                        _mapDictionary.Add(upPos, mapArrayUp);
                    }

                    if (mapArrayUp.tile == null)
                    {
                        Vector3 mapObjPosition = _grid.CellToWorld(new Vector3Int(x, 0, y));
                        mapObjPosition.x += 2f;
                        mapObjPosition.y += _wallInfo.objectSize.y;
                        mapObjPosition.z += 2f;

                        MapObject wall = ManagerHub.Instance.Pool.Pop(_wallInfo.poolType) as MapObject;
                        wall.transform.position = mapObjPosition;
                        wall.RotateObject(RotationDirection.Right);
                        wall.transform.SetParent(_tilesTrm);
                    }
                }
            }
        }

        public void TileShowOutlie(Vector2Int startPos, Vector3Int endPos)
        {
            DeleteOutline();
            _tempTileList.Clear();

            // Calculate Distance
            int maxX = Mathf.Max(startPos.x, endPos.x);
            int minX = Mathf.Min(startPos.x, endPos.x);

            int maxY = Mathf.Max(startPos.y, endPos.z);
            int minY = Mathf.Min(startPos.y, endPos.z);

            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    if (_mapDictionary.TryGetValue(new Vector2Int(x, y), out var mapArray) &&
                        mapArray.tile != null)
                    {
                        mapArray.tile.ShowOutLine(true);
                        _tempTileList.Add(mapArray.tile);
                    }
                }
            }
        }

        #endregion

        #region DeletesMethod

        public void DeleteTile()
        {
            if (_tempTileList.Count < 0) return;

            foreach (Tile tile in _tempTileList)
            {
                tile.IsFull = false;
                tile.gameObject.SetActive(false);
                tile.ShowOutLine(false);
            }

            ReBuildNavMesh();
        }

        public void DeleteOutline()
        {
            if (_tempTileList.Count <= 0) return;

            foreach (Tile tile in _tempTileList)
            {
                tile.ShowOutLine(false);
            }
        }

        #endregion

        private void ReBuildNavMesh()
        {
            _navMeshSurface.BuildNavMesh();
        }
    }
}
