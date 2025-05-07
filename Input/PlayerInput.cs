using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GM.Inputs
{
    public class PlayerInput : InputBase, Controls.IPlayerActions
    {
        public override InputType MyInputType => InputType.Player;

        public event Action OnPickEvent;

        protected override void InputSetting()
        {
            _controls.Player.SetCallbacks(this);
        }

        public override void InputEnable()
        {
            _controls.Player.Enable();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            _inputReader.SetMousePosition(context.ReadValue<Vector2>());
        }

        public void OnPick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPickEvent?.Invoke();
            }
        }

        public void OnMove(InputAction.CallbackContext context) { }
        public void OnInteract(InputAction.CallbackContext context) { }
    }
}
