using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundMove : MonoBehaviour
{
    public bool isRight;

    private RectTransform rect;

    private float width;

    private float speed;


    private void Awake()
    {
        width = Screen.width;
        speed = width / 12f;
        rect = GetComponent<RectTransform>();
        if (isRight)
        {
            rect.anchoredPosition += Vector2.right * width;
        }
    }

    public void FixedUpdate()
    {
        rect.anchoredPosition += Vector2.left * speed * Time.fixedDeltaTime;

        if (rect.anchoredPosition.x < -width) rect.anchoredPosition += Vector2.right * width * 2;
    }
}