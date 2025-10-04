using UnityEngine;

[System.Serializable]
public class CameraLimit
{
    public float left, right, top, bottom;
    private float w = 0, h = 0;
    public bool IsLimit(float x, float y)
    {
        if (w == 0) { w = Screen.width / 230f; h = Screen.height / 230f; }

        if (x < left + w) 
            return false;

        if (x > right - w)
            return false;

        if (y < bottom + h)
            return false;

        if (y > top - h)
            return false;

        return true;
    }
}
