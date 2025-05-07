using System;
using System.Collections.Generic;
using UnityEditor;
using GM.Inputs;
using UnityEngine;

public enum InputType
{
    Player,
    MapEdit,
    UI,
    Camera,
    Core,
    Shortcut,
    None
}

[CreateAssetMenu(fileName = "InputReaderSO", menuName = "SO/Input/InputReaderSO")]
public class InputReaderSO : ScriptableObject
{
    private const string INPUT_SCRIPTS_PATH = "Assets/Input";
    private const string INPUT_ASSETS_PATH = "Assets/Resources/Inputs";

    public Controls Controls => _controls;
    private Controls _controls;

    public event Action<InputType> OnInputTypeChangeEvent;

    private Dictionary<Type, InputBase> _inputDictionary = new Dictionary<Type, InputBase>();
    private Dictionary<InputType, (Action enable, Action disable)> _inputStates;

    public Vector2 MousePosition => _mousePosition;
    private Vector2 _mousePosition;

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            AddInputs();
        }
        SetInputs();
        InputsEnable();
        InitInputState();
        ChangeInputState(InputType.Player);
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void AddInputs()
    {
        _inputDictionary.Clear();

#if UNITY_EDITOR
        var guids = AssetDatabase.FindAssets("t:Script", new[] { INPUT_SCRIPTS_PATH });

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

            if (script == null) continue;

            Type type = script.GetClass();
            if (type == null || type == typeof(InputBase) || !typeof(InputBase).IsAssignableFrom(type) || type.IsAbstract)
                continue;

            try
            {
                string assetName = $"{type.Name}.asset";
                string fullPath = $"{INPUT_ASSETS_PATH}/{assetName}";

                var instance = AssetDatabase.LoadAssetAtPath<InputBase>(fullPath);
                if (instance == null)
                {
                    instance = CreateInstance(type) as InputBase;
                    if (instance != null)
                    {
                        if (!AssetDatabase.IsValidFolder(INPUT_ASSETS_PATH))
                        {
                            string[] folderPath = INPUT_ASSETS_PATH.Split('/');
                            string currentPath = folderPath[0];
                            for (int i = 1; i < folderPath.Length; i++)
                            {
                                string parent = currentPath;
                                currentPath = $"{currentPath}/{folderPath[i]}";
                                if (!AssetDatabase.IsValidFolder(currentPath))
                                {
                                    AssetDatabase.CreateFolder(parent, folderPath[i]);
                                }
                            }
                        }

                        AssetDatabase.CreateAsset(instance, fullPath);
                        AssetDatabase.SaveAssets();
                    }
                }

                if (instance != null)
                {
                    _inputDictionary.TryAdd(type, instance);
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, $"Input creation failed - Type: {type?.Name}, Error: {e.Message}");
            }
        }
#else
        var inputAssets = Resources.LoadAll<InputBase>("Inputs");
        foreach (var input in inputAssets)
        {
            if (input != null && !_inputDictionary.ContainsKey(input.GetType()))
            {
                _inputDictionary.Add(input.GetType(), input);
            }
        }
#endif

        Debug.Assert(_inputDictionary.Count > 0, $"No input instances were created or loaded.");
    }

    public T GetInput<T>() where T : InputBase
    {
        if (_inputDictionary.TryGetValue(typeof(T), out var input))
        {
            return input as T;
        }
        return null;
    }

    public void ChangeInputState(InputType type = InputType.None)
    {
        _controls.Disable();
        EnableDefaultInputs();

        if (type == InputType.None) return;

        if (_inputStates.TryGetValue(type, out var state))
        {
            state.enable();
        }

        OnInputTypeChangeEvent?.Invoke(type);
    }

    public void InputActive(InputType type, bool isActive)
    {
        if (_inputStates.TryGetValue(type, out var state))
        {
            if (isActive)
                state.enable();
            else
                state.disable();
        }
    }

    private void EnableDefaultInputs()
    {
        _inputStates[InputType.Camera].enable();
        _inputStates[InputType.Core].enable();
        _inputStates[InputType.UI].enable();
        _inputStates[InputType.Shortcut].enable();
    }

    public void SetMousePosition(Vector2 mousePos)
    {
        _mousePosition = mousePos;
    }

    private void SetInputs()
    {
        ProcessInputs(input => input.InputInitialize(this, _controls));
    }

    private void InputsEnable()
    {
        ProcessInputs(input => input.InputEnable());
    }

    private void ProcessInputs(Action<InputBase> action)
    {
        Debug.Assert(_inputDictionary != null && _inputDictionary.Count > 0, "Input dictionary is empty or null");

        foreach (var input in _inputDictionary.Values)
        {
            action(input);
        }
    }

    private void InitInputState()
    {
        _inputStates = new Dictionary<InputType, (Action enable, Action disable)>
        {
            { InputType.Player, (() => _controls.Player.Enable(), () => _controls.Player.Disable()) },
            { InputType.MapEdit, (() => _controls.MapEdit.Enable(), () => _controls.MapEdit.Disable()) },
            { InputType.UI, (() => _controls.UI.Enable(), () => _controls.UI.Disable()) },
            { InputType.Camera, (() => _controls.Camera.Enable(), () => _controls.Camera.Disable()) },
            { InputType.Core, (() => _controls.Core.Enable(), () => _controls.Core.Disable()) },
            { InputType.Shortcut, (() => _controls.ShortCut.Enable(), () => _controls.ShortCut.Disable()) }
        };
    }
}