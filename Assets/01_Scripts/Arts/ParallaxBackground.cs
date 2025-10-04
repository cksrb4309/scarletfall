using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] CameraController cam;

    [SerializeField] float BaseSpeed;

    [SerializeField] bool IsRight;

    RectTransform rect;
    
    Vector2 pos;

    float speed;
    float width;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        speed = Screen.width / 10f * BaseSpeed;

        width = Screen.width;

        pos.Set(0, 0);

        if (IsRight)
        {
            pos.x = width;

            rect.anchoredPosition = pos;
        }
    }

    private void FixedUpdate()
    {
        if (cam.Vx != 0)
        {
            pos.x += cam.Vx * speed * Time.fixedDeltaTime;

            rect.anchoredPosition = pos;

            if (rect.anchoredPosition.x > width)
            {
                pos.x -= (width * 2);

                rect.anchoredPosition = pos;
            }
            else if (rect.anchoredPosition.x < -width)
            {
                pos.x += (width * 2);

                rect.anchoredPosition = pos;
            }
        }
    }
}
