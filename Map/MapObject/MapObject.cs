using GM.Managers;
using UnityEngine;

namespace GM.Maps
{
    public enum RotationDirection
    {
        Down = 0,
        Left = 90,
        Up = 180,
        Right = -90,
        Null = -1
    }

    [RequireComponent(typeof(Outline))]
    public abstract class MapObject : MonoBehaviour, IPoolable
    {
        [Header("Setting Value")]
        public ObjectInfoSO Info => _info;
        [SerializeField] protected ObjectInfoSO _info;
        public PoolTypeSO PoolType => _poolType;
        [SerializeField] protected PoolTypeSO _poolType;
        public GameObject GameObject => gameObject;

        [Header("Visual Value")]
        [SerializeField] protected Transform _visual;

        public RotationDirection CurrentRotation => _currentRotation;
        protected RotationDirection _currentRotation = RotationDirection.Down;

        public bool IsObjectLock { get => _isObjectLock; set => _isObjectLock = value; }
        protected bool _isObjectLock = false;

        protected MeshRenderer[] _meshRenderers;
        protected Color[] _originColors;
        protected Collider _collider;
        protected Outline _outline;

        public MapObjectPart[] Parts => _parts;
        private MapObjectPart[] _parts;

        protected virtual void Awake()
        {
            _outline = GetComponent<Outline>();
            _meshRenderers = _visual.GetComponentsInChildren<MeshRenderer>();
            _collider = GetComponent<Collider>();

            _originColors = new Color[_meshRenderers.Length];
            for (int i = 0; i < _meshRenderers.Length; ++i)
            {
                _originColors[i] = _meshRenderers[i].material.color;
            }

            InitObject();

            _parts = GetComponentsInChildren<MapObjectPart>();

            for (int i = 0; i < _parts.Length; ++i)
            {
                _parts[i].Initialize(this);
            }
        }

        private void InitObject()
        {
            SetColor();
            ShowOutLine(false);
        }

        public void DestroyObject(bool isObjectPush = true)
        {
            for (int i = 0; i < _parts.Length; ++i)
            {
                _parts[i].DestoryPart();
            }

            if (isObjectPush)
            {
                ManagerHub.Instance.Pool.Push(this);
            }
        }

        public void SetTemporaryState(bool isTemp)
        {
            for (int i = 0; i < _parts.Length; ++i)
            {
                _parts[i].SetTemp(isTemp);
            }
        }

        public void SetObjectActive(bool isActive)
        {
            InitObject();
            gameObject.SetActive(isActive);
        }

        public void SetColor(MapPreviewType previewType = MapPreviewType.None)
        {
            for (int i = 0; i < _meshRenderers.Length; ++i)
            {
                Color newColor;
                newColor = previewType switch
                {
                    MapPreviewType.Delete => Color.red,
                    MapPreviewType.Relocate => Color.green,
                    _ => _originColors[i]
                };
                _meshRenderers[i].material.color = newColor;
            }
        }

        public void SetCollider(bool isActive)
        {
            _collider.enabled = isActive;
        }

        public void ShowOutLine(bool enabled)
        {
            _outline.enabled = enabled;
        }

        // TODO : 회전 로직 간소화 작업
        public void RotateObject(RotationDirection rotationDirection = RotationDirection.Null)
        {
            if (_isObjectLock) return;

            _currentRotation = rotationDirection != RotationDirection.Null
            ? rotationDirection
            : _currentRotation switch
            {
                RotationDirection.Down => RotationDirection.Left,
                RotationDirection.Left => RotationDirection.Up,
                RotationDirection.Up => RotationDirection.Right,
                RotationDirection.Right => RotationDirection.Down,
                _ => RotationDirection.Down
            };

            transform.rotation = Quaternion.Euler(0f, (float)_currentRotation, 0f);
        }

        /// <summary>
        /// Limit Y angle to 360 degrees with short value
        /// </summary>
        /// <returns></returns>
        public short GetNormalizedRotationY()
        {
            float angle = transform.localEulerAngles.y;
            angle = angle % 360;
            if (angle >= 180)
                angle -= 360;
            return (short)angle;
        }

        #region Pool

        public void SetUpPool(Pool pool)
        {
            PoolInitalize(pool);
        }

        public void ResetItem()
        {
            _isObjectLock = false;
            ResetPoolItem();
        }

        public abstract void PoolInitalize(Pool pool);
        public abstract void ResetPoolItem();

        #endregion
    }
}
