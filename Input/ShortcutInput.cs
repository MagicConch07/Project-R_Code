using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GM.Inputs
{
    public class ShortcutInput : InputBase, Controls.IShortCutActions
    {
        public override InputType MyInputType => InputType.Shortcut;

        public Action<MenuType> OpenMenuEvent;

        protected override void InputSetting()
        {
            _controls.ShortCut.SetCallbacks(this);
        }

        public override void InputEnable()
        {
            _controls.ShortCut.Enable();
        }

        public void OnOpenMapPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
                OpenMenuEvent?.Invoke(MenuType.map);
        }

        public void OnOpenPreferencePanel(InputAction.CallbackContext context)
        {
            if (context.performed)
                Debug.Log("OpenPreferencePanel");
        }

        public void OnOpenRecipePanel(InputAction.CallbackContext context)
        {
            if (context.performed)
                OpenMenuEvent?.Invoke(MenuType.recipe);
        }

        public void OnOpenStaffPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
                OpenMenuEvent?.Invoke(MenuType.staff);
        }

        public void OnOpenVendorPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
                OpenMenuEvent?.Invoke(MenuType.vendor);
        }
    }
}
