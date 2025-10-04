using System.Collections;
using UnityEngine;

public class FastDownAttackEffectParts : MonoBehaviour
{
    public SpriteRenderer sr;
    public Sprite[] sprites;

    public float minDelay;
    public float maxDelay;

    public float minSpeed;
    public float maxSpeed;

    public float minScale;
    public float maxScale;

    float scale;
    float speed;
    float delay;
    private void OnEnable()
    {
        delay = Random.Range(minDelay, maxDelay);
        speed = Random.Range(minSpeed, maxSpeed);
        scale = Random.Range(minScale, maxScale);

        transform.localScale = new Vector3(scale, scale, 1);

        StartCoroutine(ChangeCoroutine());
    }
    private void FixedUpdate()
    {
        transform.position += Vector3.up * Time.fixedDeltaTime * speed;
    }
    IEnumerator ChangeCoroutine()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sr.sprite = sprites[i];

            yield return new WaitForSeconds(delay);
        }
        Return();
    }
    void Return()
    {
        PoolingManager.Instance.ReturnObject("FastDownAttackEffectParts", gameObject);
    }
}
