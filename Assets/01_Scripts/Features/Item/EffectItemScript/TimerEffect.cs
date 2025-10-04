using System.Collections;
using UnityEngine;

public class TimerEffect : ActiveEffect
{
    public float needSecond;

    private IEnumerator PlayCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(needSecond);

            Play();
        }
    }
    public override void Enable()
    {
        StartCoroutine(PlayCoroutine());
    }
}
