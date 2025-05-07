using UnityEngine;
using GM.Entities;
using GM.GameEventSystem;
using GM.Inputs;

namespace GM.Players.Pickers
{
    public class UnitPicker : Picker
    {
        [SerializeField] protected GameEventChannelSO _uiDescriptionEventChannel;

        private Unit _pickedUnit;
        private bool _uiState;

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            _player.Input.GetInput<PlayerInput>().OnPickEvent += HandlePick;
            _uiDescriptionEventChannel.AddListener<UIDescriptionEvent>(HandleUIEvent);
        }

        private void OnDestroy()
        {
            _player.Input.GetInput<PlayerInput>().OnPickEvent -= HandlePick;
            _uiDescriptionEventChannel.RemoveListener<UIDescriptionEvent>(HandleUIEvent);
        }

        private void HandleUIEvent(UIDescriptionEvent evt)
        {
            _uiState = evt.UIState;
        }

        protected override void HandlePick()
        {
            if (_uiState) return;

            base.HandlePick();
        }

        public void NotPickUnit()
        {
            _pickedUnit = null;
        }

        protected override void PickEntity()
        {
            if (_isRay == false)
            {
                UnitUIEvents.UnitDescriptionUIEvent.isActive = false;
                _uiDescriptionEventChannel.RaiseEvent(UnitUIEvents.UnitDescriptionUIEvent);
                _pickedUnit = null;

                return;
            }

            if (_hit.transform.TryGetComponent(out Unit unit))
            {
                if (_pickedUnit == unit) return;

                UnitUIEvents.UnitDescriptionUIEvent.type = DescriptionUIType.Unit;
                UnitUIEvents.UnitDescriptionUIEvent.unit = unit;
                UnitUIEvents.UnitDescriptionUIEvent.isActive = true;
                _uiDescriptionEventChannel.RaiseEvent(UnitUIEvents.UnitDescriptionUIEvent);

                _pickedUnit = unit;
            }
        }
    }
}
