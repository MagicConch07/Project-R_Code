using UnityEngine;

namespace GM.InteractableEntities
{
    public class DividedTableEntity : InteractableEntity
    {
        public Transform SenderTransform => _senderTransform;
        [SerializeField] protected Transform _senderTransform;

        public Transform ReceiverTransform => _receiverTransform;
        [SerializeField] protected Transform _receiverTransform;
    }
}
