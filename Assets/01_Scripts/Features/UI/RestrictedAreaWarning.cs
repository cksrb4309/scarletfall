using System.Collections;
using UnityEngine;

public class RestrictedAreaWarning : MonoBehaviour
{
    public SpriteRenderer warningSr;
    public WallAttack wallAttack;
    Color srColor = Color.white;

    float Alpha
    {
        set
        {
            srColor.a = value;

            warningSr.color = srColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();

            StartCoroutine(WarningCoroutine());
        }
    }

    IEnumerator WarningCoroutine()
    {
        double t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 0.5f;

            Alpha = (float)t;

            yield return null;
        }

        // 오래 있었을 경우 조치 여기다가~
        wallAttack.StartAttack();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();

            wallAttack.EndAttack();

            StartCoroutine(StopWarningCoroutine());
        }
    }
    IEnumerator StopWarningCoroutine()
    {
        double t = srColor.a;

        while (t > 0)
        {
            t -= Time.deltaTime * 1f;

            Alpha = (float)t;

            yield return null;
        }
    }
}
