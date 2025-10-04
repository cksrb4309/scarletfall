using UnityEngine;

public class SlimeSearchCollider : MonoBehaviour
{
    public Slime slime;

    private float attackDelay = 1f;

    private float t = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && t > attackDelay)
        {
            if (slime.transform.position.x > transform.position.x)
                slime.LeftAttack();
            else
                slime.RightAttack();
            t = 0f;
        }
    }
    private void FixedUpdate()
    {
        t += Time.fixedDeltaTime;
    }
}