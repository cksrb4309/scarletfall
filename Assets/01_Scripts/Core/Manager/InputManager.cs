using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;
public class InputManager : MonoBehaviour
{
    static InputManager instance = null;

    [SerializeField] SerializedDictionary<InputType, InputActionReference> inputActions;

    Dictionary<InputType, int> inputActionCounts = new Dictionary<InputType, int>();

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }
    public static InputActionReference GetInputAction(InputType inputType)
    {
        if (!instance.inputActions.ContainsKey(inputType))
        {
            Debug.LogWarning("요청한 InputType에 맞는 InputAction이 없습니다 ! : " + inputType.ToString());

            return null;
        }
        if (instance.inputActionCounts.ContainsKey(inputType)) instance.inputActionCounts[inputType]++;

        else instance.inputActionCounts[inputType] = 1;

        instance.inputActions[inputType].action.Enable();

        return instance.inputActions[inputType];
    }
    public static void Release(InputType inputType)
    {
        if (!instance.inputActions.ContainsKey(inputType))
        {
            Debug.LogWarning("해제 요청한 InputType에 맞는 InputAction이 없습니다 ! : " + inputType.ToString());

            return;
        }
        if (--instance.inputActionCounts[inputType] == 0) instance.inputActions[inputType].action.Disable();
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
}