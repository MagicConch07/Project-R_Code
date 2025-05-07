using UnityEngine;

namespace GM.InteractableEntities
{
    public class SingleTableEntity : InteractableEntity
    {
        public Transform EntityTransform => _entityTransform;
        [SerializeField] protected Transform _entityTransform;
    }
}
