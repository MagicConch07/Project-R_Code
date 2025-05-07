namespace GM.Maps
{
    public class MapArray
    {
        public MapObjectPart MapPart => _mapPart;
        public bool IsTemp => _isTemp;

        public Tile tile;
        private bool _isTemp = false;

        private MapObjectPart _mapPart;

        public void SetMapPart(MapObjectPart mapPart)
        {
            _mapPart = mapPart;
            _isTemp = false;

            _mapPart.OnDestoryObjectEvent += HandleObjectDestory;
            _mapPart.OnChangeTempObject += HandleChangeTempObject;
        }

        private void HandleChangeTempObject(bool isTemp)
        {
            _isTemp = isTemp;
        }

        private void HandleObjectDestory()
        {
            _mapPart = null;
            _isTemp = false;
        }
    }
}
