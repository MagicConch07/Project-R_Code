using System;
using UnityEngine;

namespace GM.Entities
{
    public class EntityAnimatorTrigger : MonoBehaviour, IEntityComponent
    {
        public event Action OnAnimationEnd;
        public event Action OnEventAnimationEnd;
        public event Action OnCookAnimationStarted;
        public event Action OnCookAnimationEnd;

        protected Entity _entity;

        public void Initialize(Entity entity)
        {
            _entity = entity;
        }

        protected virtual void AnimationEnd() => OnAnimationEnd?.Invoke();
        protected virtual void EventAnimationEnd() => OnEventAnimationEnd?.Invoke();
        protected virtual void CookAnimationStarted() => OnCookAnimationStarted?.Invoke();
        protected virtual void CookAnimationEnd() => OnCookAnimationEnd?.Invoke();
    }
}
