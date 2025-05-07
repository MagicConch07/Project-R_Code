using UnityEngine;
using UnityEngine.AI;

namespace GM.Entities
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Unit : Entity
    {
        public NavMeshAgent NavAgent => _navAgent;
        protected NavMeshAgent _navAgent;

        public bool CanManualMove => _canManualMove;
        protected bool _canManualMove = false;

        protected override void Awake()
        {
            base.Awake();
            _navAgent = GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Set Movement Destination
        /// </summary>
        /// <param name="targetTrm">target Transform to move</param>
        public virtual void SetMovement(Transform targetTrm)
        {
            _navAgent.SetDestination(targetTrm.position);
        }

        /// <summary>
        /// Move Stop
        /// </summary>
        public void StopImmediately()
        {
            _navAgent.isStopped = true;
        }
    }
}
