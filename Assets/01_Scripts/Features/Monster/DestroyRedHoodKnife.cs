using System.Collections;
using UnityEngine;

public class DestroyRedHoodKnife : MonoBehaviour
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public float torque = 360f;

    private void OnEnable()
    {
        rb.AddForce(Vector3.up * 3f, ForceMode2D.Impulse);
        rb.AddTorque(torque);
        sr.color = Color.white;
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        Color color = Color.white;
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * 2f;
            color.a = t;
            sr.color = color;
            yield return null;
        }
        PoolingManager.Instance.ReturnObject("DestroyRedHoodKnife", gameObject);
    }
}
