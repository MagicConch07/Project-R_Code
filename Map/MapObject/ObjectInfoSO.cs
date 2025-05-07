using UnityEngine;

namespace GM.Maps
{
    [CreateAssetMenu(fileName = "ObjectInfoSO", menuName = "SO/Map/Object")]
    public class ObjectInfoSO : ScriptableObject
    {
        [Header("Information")]
        public Enums.InteractableEntityType type;
        public PoolTypeSO poolType;
        public string objectName;
        public string displayName;
        public Sprite displayImage;
        public int cost;
        public Vector3 objectSize = new Vector3(1, 0.5f, 1);
        public Vector2Int cellSize;
        public bool unLock;
    }
}
