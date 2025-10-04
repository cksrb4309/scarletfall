using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LineUI : MonoBehaviour
{
    public AnimationCurve speedCurve;
    public RectTransform lineImage;
    float width;
    Vector2 currentSize;
    float t;
    private void Start()
    {
        width = lineImage.sizeDelta.x;

        lineImage.sizeDelta = new Vector2(0, lineImage.sizeDelta.y);

        currentSize = new Vector2(0, lineImage.sizeDelta.y);

        lineImage.sizeDelta = currentSize;
    }

    public void Increase()
    {
        StopAllCoroutines();

        StartCoroutine(IncreaseCoroutine());
    }
    IEnumerator IncreaseCoroutine()
    {
        while (t < 1f)
        {
            t += Time.deltaTime * speedCurve.Evaluate(t);

            currentSize.x = Mathf.Lerp(0, width, t);

            lineImage.sizeDelta = currentSize;

            yield return null;
        }
    }
    public void Decrease() 
    {
        StopAllCoroutines();

        StartCoroutine(DecreaseCoroutine());
    }
    IEnumerator DecreaseCoroutine()
    {
        while (t > 0f)
        {
            t -= Time.deltaTime * speedCurve.Evaluate(t);

            currentSize.x = Mathf.Lerp(0, width, t);

            lineImage.sizeDelta = currentSize;

            yield return null;
        }
    }
}
