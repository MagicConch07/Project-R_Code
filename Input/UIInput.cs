using System;
using UnityEngine.InputSystem;

namespace GM.Inputs
{
    public class UIInput : InputBase, Controls.IUIActions
    {
        public override InputType MyInputType => InputType.UI;

        public event Action OnUIClickEvent;

        protected override void InputSetting()
        {
            _controls.UI.SetCallbacks(this);
        }

        public override void InputEnable()
        {
            _controls.UI.Enable();
        }

        public void OnCancel(InputAction.CallbackContext context) { }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnUIClickEvent?.Invoke();
            }
        }

        public void OnMiddleClick(InputAction.CallbackContext context) { }

        public void OnNavigate(InputAction.CallbackContext context) { }

        public void OnPoint(InputAction.CallbackContext context) { }

        public void OnRightClick(InputAction.CallbackContext context) { }

        public void OnScrollWheel(InputAction.CallbackContext context) { }

        public void OnSubmit(InputAction.CallbackContext context) { }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
    }
}
