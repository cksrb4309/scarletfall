using UnityEngine;

public class SparkLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float duration = 2.0f; // 알파 값이 감소하는 데 걸리는 시간

    private float t = 1f;
    private Gradient originalGradient;

    void OnEnable()
    {
        t = 1f;

        // LineRenderer 설정 가져오기
        lineRenderer = GetComponent<LineRenderer>();

        // 기존에 지정한 색상을 유지하기 위해 초기 색상 값을 저장
        originalGradient = lineRenderer.colorGradient;
    }

    void FixedUpdate()
    {
        t -= Time.fixedDeltaTime;
        float alpha = Mathf.Lerp(0, 1f, t);

        Gradient newGradient = new Gradient();

        GradientColorKey[] colorKeys = originalGradient.colorKeys;
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[colorKeys.Length];

        // 기존 색상의 알파 값을 감소시킴
        for (int i = 0; i < colorKeys.Length; i++)
        {
            alphaKeys[i] = new GradientAlphaKey(alpha, colorKeys[i].time);
        }

        newGradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = newGradient;
    }
}
