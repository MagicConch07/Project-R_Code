using GM.InteractableEntities;
using UnityEngine;

namespace GM.CookWare
{
    public class CookingTable : SingleTableEntity
    {
        public AnimationClip CookAnimation => _cookAnimation;
        [SerializeField] protected AnimationClip _cookAnimation;
    }
}
