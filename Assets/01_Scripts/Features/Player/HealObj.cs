using System.Collections;
using UnityEngine;

public class HealObj : MonoBehaviour
{
    public float healValue;
    public float height = 0.5f;
    Vector3 Player { get { return GameManager.Instance.player.transform.position + Vector3.up * height; } }
    Vector3 velocity = Vector3.zero;
    public void Setting(float healvalue)
    {
        this.healValue = healvalue;

        velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 2;

        StartCoroutine(DecreaseVelocityCoroutine());
    }
    IEnumerator DecreaseVelocityCoroutine()
    {
        Vector3 startVelocity = velocity;
        Vector3 endVelocity = Vector3.zero;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 0.5f;

            transform.position += Vector3.Lerp(startVelocity, endVelocity, t) * Time.deltaTime;

            yield return null;
        }
        StartCoroutine(TargetTrackingCoroutine());
    }
    IEnumerator TargetTrackingCoroutine()
    {
        Vector3 start = transform.position;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;

            transform.position = Vector3.Lerp(start, Player, t);

            yield return null;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.instance.Hit(-healValue);

            PoolingManager.Instance.ReturnObject("HealObj", gameObject);
        }
    }
}
