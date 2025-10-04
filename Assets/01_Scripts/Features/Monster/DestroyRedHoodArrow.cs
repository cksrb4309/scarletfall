using System.Collections.Generic;
using UnityEngine;

public class DestroyRedHoodArrow : MonoBehaviour
{
    public Rigidbody2D head;
    public Rigidbody2D tail;

    private void OnEnable()
    {
        bool r = Random.value > 0.5f;

        head.transform.position = transform.position;
        tail.transform.position = transform.position;

        head.linearVelocity = Vector2.zero;
        tail.linearVelocity = Vector2.zero;

        head.rotation = 0;
        tail.rotation = 0;

        head.AddTorque(r ? -360 : 360);
        tail.AddTorque(r ? 360 : -360);

        Vector3 dir;
        dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        dir.Normalize();

        head.AddForce(dir * 2f, ForceMode2D.Impulse);
        tail.AddForce(dir * -2f, ForceMode2D.Impulse);
    }
}