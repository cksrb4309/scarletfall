using System.Collections;
using UnityEngine;

public class ArmoroftheVoid : StackEffect
{
    public SpriteRenderer shield;

    Color color;

    public override void Play()
    {
        shield.gameObject.SetActive(true);

        color = shield.color;
        PlayerFlags.Value.Shield = true;

        StopAllCoroutines();

        StartCoroutine(ShieldCoroutine());
    }
    IEnumerator ShieldCoroutine()
    {
        yield return new WaitForSeconds(2f);

        shield.color = new Color(0, 0, 0, 0);

        yield return new WaitForSeconds(0.25f);

        shield.color = color;

        yield return new WaitForSeconds(0.25f);

        shield.color = new Color(0, 0, 0, 0);

        yield return new WaitForSeconds(0.25f);

        shield.color = color;

        yield return new WaitForSeconds(0.25f);

        shield.gameObject.SetActive(false);

        PlayerFlags.Value.Shield = false;
    }
}
