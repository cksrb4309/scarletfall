using UnityEngine;

public class AlphaBlink : MonoBehaviour
{
    public SpriteRenderer sr;

    public float max = 0.8f;
    public float min = 0.6f;
    public float speed;

    private float Alpha {
        get { return alpha; }
        set { alpha = value; sr.color = new Color(1, 1, 1, alpha); }
    }
    private float alpha;
    private bool isCheck = false;
    private bool dir = false;
    // dir는 값의 방향으로 true일 때는 증가 false일 때는 알파값을 감소시킴

    public void StartAnimation()
    {
        Alpha = 0;
        isCheck = true;
        dir = true;
    }
    public void EndAnimation()
    {
        isCheck = false;
        dir = false;
    }
    private void OnDisable()
    {
        isCheck = false;
        dir = false;
    }

    private void FixedUpdate()
    {
        if (isCheck)
        {
            if (dir)
            {
                if (Alpha < max)
                {
                    Alpha += Time.fixedDeltaTime * speed;
                }
                else
                {
                    dir = false;
                }
            }
            else
            {
                if (Alpha > min)
                {
                    Alpha -= Time.fixedDeltaTime * speed;
                }
                else
                {
                    dir = true;
                }
            }
        }
        else
        {
            if (Alpha > 0)
            {
                Alpha -= Time.fixedDeltaTime * speed;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartAnimation();
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EndAnimation();
        }
    }
}
