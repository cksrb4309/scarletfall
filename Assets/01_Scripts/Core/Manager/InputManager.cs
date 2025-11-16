using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;
public class InputManager : Singleton<InputManager>
{
    [SerializeField] SerializedDictionary<InputType, InputActionReference> inputActions;

    Dictionary<InputType, int> inputActionCounts = new Dictionary<InputType, int>();

    public static InputActionReference GetInputAction(InputType inputType)
    {
        if (!Instance.inputActions.ContainsKey(inputType))
        {
            Debug.LogWarning("요청한 InputType에 맞는 InputAction이 없습니다 ! : " + inputType.ToString());

            return null;
        }
        if (Instance.inputActionCounts.ContainsKey(inputType)) Instance.inputActionCounts[inputType]++;

        else Instance.inputActionCounts[inputType] = 1;

        Instance.inputActions[inputType].action.Enable();

        return Instance.inputActions[inputType];
    }
    public static void Release(InputType inputType)
    {
        if (instance == null) return;
        
        if (!instance.inputActions.ContainsKey(inputType)) return;
        
        if (--instance.inputActionCounts[inputType] == 0)
        {
            instance.inputActions[inputType].action.Disable();
        }
    }
    private void OnDisable()
    {
        foreach (var kvp in inputActionCounts)
        {
            if (kvp.Value > 0)
            {
                inputActions[kvp.Key].action.Disable();
            }
        }
    }
}

public enum InputType
{
    LeftMove,
    RightMove,
    Jump,
    Down,
    Attack,
    Roll,
    Interact,
    Escape,
}