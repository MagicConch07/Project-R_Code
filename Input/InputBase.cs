using UnityEngine;

namespace GM.Inputs
{
    public class InputBase : ScriptableObject
    {
        public virtual InputType MyInputType => _myInputType;
        [SerializeField] protected InputType _myInputType;

        protected InputReaderSO _inputReader;
        protected Controls _controls;

        public virtual void InputEnable() { }
        protected virtual void InputSetting() { }

        public void InputInitialize(InputReaderSO inputReader, Controls controlls)
        {
            _inputReader = inputReader;
            _controls = controlls;
            InputSetting();
        }
    }
}
