using UnityEngine;

public static class RotationExtensions2D
{
    public static float GetLookRotation2D(this Vector3 from, Vector3 to)
    {
        Vector2 dir = to - from;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
    public static float GetLookRotation2D(this Vector2 from, Vector2 to)
    {
        Vector2 dir = to - from;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
