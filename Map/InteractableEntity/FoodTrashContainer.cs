using UnityEngine;

namespace GM.InteractableEntities
{
    public class FoodTrashContainer : SingleTableEntity
    {
        public Transform FoodTrashTransform => _foodTrashTransform;
        [SerializeField] private Transform _foodTrashTransform;
    }
}
