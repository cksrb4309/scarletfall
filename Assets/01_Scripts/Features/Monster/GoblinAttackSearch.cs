using UnityEngine;

public class GoblinAttackSearch : MonoBehaviour
{
    public Goblin goblin;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) goblin.Attack();
    }
}
