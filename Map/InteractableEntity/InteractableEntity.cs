using GM.Maps;
using UnityEngine;

namespace GM.InteractableEntities
{
    public class InteractableEntity : MapObject
    {
        public bool InUse { get => _inUse; set => _inUse = value; }
        protected bool _inUse = false;

        public Enums.InteractableEntityType Type => _info.type;

        public bool IsShared => _isShared;
        [SerializeField] private bool _isShared;

        public override void PoolInitalize(Pool pool)
        {

        }

        public override void ResetPoolItem()
        {

        }
    }
}
