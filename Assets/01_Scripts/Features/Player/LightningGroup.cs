using System.Collections;
using UnityEngine;

public class LightningGroup : MonoBehaviour
{
    public LineRenderer[] lr;
    private void Awake()
    {
        for (int i = 0; i < lr.Length; i++)
        {
            lr[i].enabled = false;

            lr[i].useWorldSpace = true;

            lr[i].positionCount = 2;
        }
    }

    public void Set(Vector3 start, Vector3 end)
    {
        for (int i = 0; i < lr.Length; i++)
        {
            lr[i].enabled = true;

            lr[i].SetPosition(0, start);
            lr[i].SetPosition(1, end);
        }

        StartCoroutine(AnimationCoroutine());
    }
    IEnumerator AnimationCoroutine()
    {
        yield return new WaitForSeconds(0.15f);
        lr[0].enabled = false;
        yield return new WaitForSeconds(0.15f);
        lr[1].enabled = false;
        yield return new WaitForSeconds(0.1f);
        lr[2].enabled = false;
    }
}
