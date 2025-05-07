using UnityEngine;

namespace GM.InteractableEntities
{
    public class FoodOut : DividedTableEntity
    {
        public Transform FoodTrm => _foodTrm;
        [SerializeField] private Transform _foodTrm;
    }
}
