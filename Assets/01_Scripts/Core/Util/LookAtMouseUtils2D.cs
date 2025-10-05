using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class LookAtMouseUtils2D : Singleton<LookAtMouseUtils2D>
{
    InputActionReference mousePositionInput = null;
    public static float GetLookAtMouseAngle(Vector3 st)
    {
        Vector3 mouseScreenPos = Instance.mousePositionInput.action.ReadValue<Vector2>();
        Vector2 dir = mouseScreenPos - st;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
    private void OnEnable()
    {
        //mousePositionInput = InputManager.GetInputAction(InputType.MousePoint);
    }
    private void OnDisable()
    {
        //InputManager.Release(InputType.MousePoint);
    }
}
