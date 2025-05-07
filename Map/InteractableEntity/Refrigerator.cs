using GM.CookWare;
using GM.Entities;
using GM.Staffs;
using UnityEngine;

namespace GM.InteractableEntities
{
    public class Refrigerator : CookingTable
    {
        [SerializeField] private Animator _animator;

        private Chef _chef;
        private EntityAnimatorTrigger _animatorTrigger;

        public void SetChef(Chef chef)
        {
            _chef = chef;
            _animatorTrigger = _chef.GetCompo<EntityAnimatorTrigger>();

            _animatorTrigger.OnCookAnimationStarted += HandleCookAnimationStarted;
            _animatorTrigger.OnCookAnimationEnd += HandleCookAnimationEnd;
            _animatorTrigger.OnAnimationEnd += HandleAnimationEnd;
        }

        private void ClearChef()
        {
            _chef = null;
            _animatorTrigger.OnCookAnimationStarted -= HandleCookAnimationStarted;
            _animatorTrigger.OnCookAnimationEnd -= HandleCookAnimationEnd;
            _animatorTrigger.OnAnimationEnd -= HandleAnimationEnd;
            _animatorTrigger = null;

            _animator.SetBool("OPEN", false);
            _animator.SetBool("CLOSE", false);
        }

        private void HandleCookAnimationStarted()
        {
            _animator.SetBool("OPEN", true);
        }

        private void HandleCookAnimationEnd()
        {
            _animator.SetBool("OPEN", false);
            _animator.SetBool("CLOSE", true);
        }

        private void HandleAnimationEnd()
        {
            ClearChef();
        }
    }
}
