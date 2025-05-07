using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GM.Inputs
{
    public class CameraInput : InputBase, Controls.ICameraActions
    {
        public override InputType MyInputType => InputType.Camera;

        public Action<int> OnMouseWheel;
        public Action<bool> OnMouseClick;
        public Vector2 MousePos;

        protected override void InputSetting()
        {
            _controls.Camera.SetCallbacks(this);
        }

        public override void InputEnable()
        {
            _controls.Camera.Enable();
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnMouseWheel?.Invoke((int)context.ReadValue<Vector2>().y);
        }

        public void OnPos(InputAction.CallbackContext context)
        {
            MousePos = context.ReadValue<Vector2>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.started)
                OnMouseClick?.Invoke(true);
            else if (context.performed)
            {
                
            }
            else if (context.canceled)
                OnMouseClick?.Invoke(false);
        }
    }
}
