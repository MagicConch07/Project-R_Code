using UnityEngine.InputSystem;

namespace GM.Inputs
{
    public class CoreInput : InputBase, Controls.ICoreActions
    {
        public override InputType MyInputType => InputType.Core;

        private bool _isMapEdit = false;

        protected override void InputSetting()
        {
            _controls.Core.SetCallbacks(this);
        }

        public override void InputEnable()
        {
            _controls.Core.Enable();
        }

        public void SetMapEditChange()
        {
            _isMapEdit = !_isMapEdit;

            if (_isMapEdit)
            {
                _inputReader.ChangeInputState(InputType.MapEdit);
            }
            else
            {
                _inputReader.ChangeInputState(InputType.Player);
            }
        }

        public void OnMapEditChange(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SetMapEditChange();
            }
        }
    }
}
