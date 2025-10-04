using System.Collections;
using UnityEngine;

public class ShieldGroup : MonoBehaviour
{
    public Transform[] shield;
    public float[] t;
    public float speed;

    Vector3 startAngle = new Vector3(0, -90, 0);
    Vector3 endAngle = new Vector3(0, 90, 0);

    Vector3 startPos = new Vector3(0.7f, 0, 0);
    Vector3 startBasePos = new Vector3(0.7f, 0, 0);

    Vector3 endPos = new Vector3(-0.7f, 0, 0);
    Vector3 endBasePos = new Vector3(-0.7f, 0, 0);

    Vector3 activeStartPos = new Vector3(5f, 0, 0);
    Vector3 activeEndPos = new Vector3(5f, 0, 0);

    bool isEnabled = false;
    public void Enable()
    {
        // 재생 시작
        for (int i = 0; i < shield.Length; i++) shield[i].gameObject.SetActive(true);

        isEnabled = true;

        StartCoroutine(EnableCoroutine());
    }
    IEnumerator EnableCoroutine()
    {
        float t = 0;
        startPos = activeStartPos;
        endPos = activeEndPos;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;

            startPos = Vector3.Lerp(activeStartPos, startBasePos, t);
            endPos = Vector3.Lerp(activeEndPos, endBasePos, t);

            yield return null;
        }
        startPos = startBasePos;
        endPos = endBasePos;
    }
    public void Diasable()
    {
        // 재생 종료
        StartCoroutine(DiasableCoroutine());
    }
    IEnumerator DiasableCoroutine()
    {
        float t = 0;
        startPos = startBasePos;
        endPos = endBasePos;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;

            startPos = Vector3.Lerp(startBasePos, activeStartPos, t);
            endPos = Vector3.Lerp(endBasePos, activeEndPos, t);

            yield return null;
        }
        startPos = activeStartPos;
        endPos = activeEndPos;


        for (int i = 0; i < shield.Length; i++) shield[i].gameObject.SetActive(false);

        isEnabled = false;
    }
    private void FixedUpdate()
    {
        if (isEnabled)
        {
            for (int i = 0; i < t.Length; i++)
            {
                t[i] += speed * Time.fixedDeltaTime;

                if ((int)(t[i] % 2) == 1) // 값이 홀수일 때
                {
                    shield[i].eulerAngles = Vector3.Lerp(startAngle, endAngle, t[i] % 1);
                    shield[i].localPosition = Vector3.Lerp(startPos, endPos, t[i] % 1);
                }
                else // 값이 짝 수일 때
                {
                    shield[i].eulerAngles = startAngle;
                    shield[i].localPosition = startPos;
                }
            }
        }
    }
}
