using System.Collections;
using UnityEngine;

public class RedHoodKnife : MonoBehaviour
{
    public Collider2D cd;
    public float destroyDelay = 10f;
    public float speed = 5f;
    public Vector3 dir = Vector3.zero;

    public void StartMove(float speed, Vector3 dir)
    {
        this.speed = speed;
        this.dir = dir;

        // 방향 벡터의 각도를 구함
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Z축을 기준으로 회전값 넣음
        transform.rotation = Quaternion.Euler(0, 0, angle);

        StartCoroutine(ReturnCoroutine());
    }
    IEnumerator ReturnCoroutine()
    {
        yield return new WaitForSeconds(destroyDelay);

        PoolingManager.Instance.ReturnObject("RedHoodKnife", gameObject);
    }
    private void FixedUpdate()
    {
        transform.position += dir * Time.fixedDeltaTime * speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSwing"))
        {
            cd.enabled = false;

            GameObject knife = PoolingManager.Instance.GetObject("DestroyRedHoodKnife");
            knife.transform.position = transform.position;
            knife.transform.rotation = transform.rotation;

            PoolingManager.Instance.ReturnObject("RedHoodKnife", gameObject);
        }
    }
}
