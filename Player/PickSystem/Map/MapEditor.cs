using GM.Maps;
using UnityEngine;

namespace GM.Players.Pickers.Maps
{
    public enum MapEditType
    {
        Select,
        Create,
        Relocator,
        Delete,
        Move,
        Rotate,
    }

    public abstract class MapEditor : MonoBehaviour
    {
        public abstract MapEditType EditType { get; }

        protected Map _map;
        protected MapPicker _mapPicker;
        protected ObjectInfoSO _mapObjectInfo;

        public virtual void StartEdit()
        {
            // Intentionally Empty
        }

        public virtual void EndEdit()
        {
            // Intentionally Empty
        }

        public void Initialize(MapPicker mapPicker, Map map)
        {
            _mapPicker = mapPicker;
            _map = map;
            _mapPicker.SetObjectInfoEvent += HandleSetMapObjectInfo;
            _mapPicker.OnChangeMapEditTypeEvent += HandleChangeMapEditType;
        }

        protected virtual void HandleSetMapObjectInfo(ObjectInfoSO mapObjectInfo)
        {
            _mapObjectInfo = mapObjectInfo;
        }

        protected virtual void HandleChangeMapEditType(MapEditType type)
        {
            if (EditType != type)
            {
                EndEdit();
            }
        }
    }
}
