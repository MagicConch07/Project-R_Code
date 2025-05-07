using GM.Entities;
using UnityEngine;

namespace GM.Players.Pickers
{
    public abstract class Picker : MonoBehaviour, IEntityComponent
    {
        [SerializeField] protected LayerMask _pickLayer;
        [SerializeField] protected Camera _pickCam;

        protected Player _player;
        protected RaycastHit _hit;
        protected bool _isRay;

        public virtual void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        protected virtual void HandlePick()
        {
            PickRaycast();
            PickEntity();
        }

        protected void PickRaycast()
        {
            Ray ray = _pickCam.ScreenPointToRay(_player.Input.MousePosition);
            _isRay = Physics.Raycast(ray, out _hit, _pickCam.farClipPlane, _pickLayer);
        }

        protected abstract void PickEntity();
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            if (_player == null) return;
            var ray = _pickCam.ScreenPointToRay(_player.Input.MousePosition);
            Vector3 endPoint = ray.origin + ray.direction * _pickCam.farClipPlane;
            Gizmos.DrawRay(ray.origin, endPoint);
        }
#endif
    }
}
