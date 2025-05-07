using GM.Entities;
using UnityEngine;

namespace GM
{
    public class ChefAnimator : EntityAnimator
    {
        [SerializeField] private AnimationClip _originalChangeClip;
        private AnimatorOverrideController _animatorOverrideController;

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            _animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _animatorOverrideController;
        }

        public void SetCookingAnimation(AnimationClip nextClip, float speed)
        {
            _animatorOverrideController[_originalChangeClip.name] = nextClip;
            SetAnimationSpeed("CookingSpeed", speed);
        }
    }
}
