using UnityEngine;

public class PoisonApple : MonoBehaviour
{
    public Rigidbody2D rb;
    public float randomX;

    public void OnEnable()
    {
        rb.linearVelocity = Vector3.zero;

        rb.AddForce(new Vector2(Random.Range(-randomX, randomX), 9f), ForceMode2D.Impulse);

        rb.angularVelocity = 0;

        rb.AddTorque(180f);
    }
}
