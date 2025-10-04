using System.Collections;
using UnityEngine;

public class WallAttack : MonoBehaviour
{
    public Collider2D cd;
    public void StartAttack()
    {
        StartCoroutine(StartAttackCoroutine());
    }
    public void EndAttack()
    {
        StopAllCoroutines();

        cd.enabled = false;
    }

    IEnumerator StartAttackCoroutine()
    {
        while (true)
        {
            cd.enabled = true;
            yield return new WaitForSeconds(0.5f);
            cd.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
