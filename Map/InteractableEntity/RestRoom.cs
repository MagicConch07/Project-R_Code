using GM.GameEventSystem;
using GM.Maps;
using UnityEngine;

namespace GM.InteractableEntities
{
    public class RestRoom : InteractableEntity, IMapVisualer
    {
        [SerializeField] private GameEventChannelSO _mapUIEventChannel;

        [SerializeField] private StaffRestPart[] _restTrm;

        // TODO : 보류

        public void ActiveMapVisual(bool isActive)
        {
            foreach (var rest in _restTrm)
            {
                rest.gameObject.SetActive(isActive);
            }
        }
    }
}
