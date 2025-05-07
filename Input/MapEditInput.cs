using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GM.Inputs
{
    public class MapEditInput : InputBase, Controls.IMapEditActions
    {
        public override InputType MyInputType => InputType.MapEdit;

        public event Action<bool> OnMapDragEvent;
        public event Action<bool> OnMapEditLeftClickEvent;
        public event Action OnRotateObjectEvent;
        public event Action MapEditTypeChangeEvent;
        public event Action CancelMapObjectEvent;

        private bool _isDrag = false;

        protected override void InputSetting()
        {
            _controls.MapEdit.SetCallbacks(this);
        }

        public override void InputEnable()
        {
            _controls.MapEdit.Enable();
        }

        public void OnMapPoint(InputAction.CallbackContext context)
        {
            _inputReader.SetMousePosition(context.ReadValue<Vector2>());

            if (context.performed)
            {
                if (_isDrag == true)
                {
                    // TODO : Delete Drag Event
                    OnMapDragEvent?.Invoke(true);
                }
            }
        }

        public void OnMapEditLeftClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnMapEditLeftClickEvent?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnMapEditLeftClickEvent?.Invoke(false);
            }
        }

        public void OnMapDrag(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isDrag = true;
            }
            else if (context.canceled)
            {
                _isDrag = false;
                OnMapDragEvent?.Invoke(false);
            }
        }

        public void OnRotateObject(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnRotateObjectEvent?.Invoke();
            }
        }

        public void OnMapEditTypeChange(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                MapEditTypeChangeEvent?.Invoke();
            }
        }

        public void OnCancelMapObject(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CancelMapObjectEvent?.Invoke();
            }
        }
    }
}
