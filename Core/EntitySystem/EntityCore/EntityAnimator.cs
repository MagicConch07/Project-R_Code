using System;
using UnityEngine;

namespace GM.Entities
{
    public class EntityAnimator : MonoBehaviour, IEntityComponent
    {
        protected Entity _entity;
        protected Animator _animator;

        public virtual void Initialize(Entity entity)
        {
            _entity = entity;
            _animator = GetComponent<Animator>();
        }

        public void SetAnimationSpeed(string animName, float speed)
        {
            _animator.SetFloat(animName, speed);
        }
    }
}
