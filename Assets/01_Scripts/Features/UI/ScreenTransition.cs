using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour
{
    static ScreenTransition instance = null;

    public ScreenTransitionData[] TransitionsData;

    public Transform canvasTransform;

    public Image fadeImage;

    private Dictionary<string, UIParticle> particleDict = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 시작 전환이 끝난 경우 함수를 실행하고, 마무리 전환을 진행한다
    /// </summary>
    /// <param name="startTransition">시작 화면전환 명칭</param>
    /// <param name="endTransition">종료 화면전환 명칭</param>
    /// <param name="action">시작 화면전환이 종료된 후 실행될 액션</param>
    /// <param name="fadeStart">이미지의 Alpha 적용 지연 시간</param>
    /// <param name="fadeEnd">이미지의 Alpha 적용 지연 시간</param>
    public static void Play(string startTransition, string endTransition, Action action, float fadeStart = 0, float fadeEnd = 1)
    {
        instance.Excute(startTransition, endTransition, action, fadeStart, fadeEnd);
    }

    /// <summary>
    /// 시작 전환이 끝난 경우 씬을 이동하고, 마무리 전환을 진행한다
    /// </summary>
    /// <param name="startTransition">시작 화면전환 명칭</param>
    /// <param name="endTransition">종료 화면전환 명칭</param>
    /// <param name="sceneName">이동할 씬 명칭</param>
    /// <param name="fadeStart">이미지의 Alpha 적용 지연 시간</param>
    /// <param name="fadeEnd">이미지의 Alpha 적용 지연 시간</param>
    public static void Play(string startTransition, string endTransition, string sceneName, float fadeStart = 0, float fadeEnd = 1)
    {
        instance.Excute(startTransition, endTransition, sceneName, fadeStart, fadeEnd);
    }

    /// <summary>
    /// 시작 전환이 끝난 경우 함수를 실행하고, duration 동안 지연한 후에 마무리 전환을 진행한다
    /// </summary>
    /// <param name="startTransition">시작 화면전환 명칭</param>
    /// <param name="endTransition">종료 화면전환 명칭</param>
    /// <param name="action">시작 화면전환이 종료된 후 실행될 액션</param>
    /// <param name="fadeStart">이미지의 Alpha 적용 지연 시간</param>
    /// <param name="fadeEnd">이미지의 Alpha 적용 지연 시간</param>
    /// <param name="duration"></param>
    public static void Play(string startTransition, string endTransition, Action action, float fadeStart = 0, float fadeEnd = 1, float duration = 1)
    {
        instance.Excute(startTransition, endTransition, action, fadeStart, fadeEnd, duration);
    }
    public void Excute(string startTransition, string endTransition, string sceneName, float fadeStart, float fadeEnd)
    {
        ScreenTransitionData st, ed;

        GetTransition(ref startTransition, out st);
        GetTransition(ref endTransition, out ed);

        fadeImage.color = st.FadeImageColor;

        StartCoroutine(FadeCoroutine(fadeStart, fadeEnd, st.Length, ed.Length, sceneName, st, ed));
    }
    public void Excute(string startTransition, string endTransition, Action action, float fadeStart, float fadeEnd)
    {
        ScreenTransitionData st, ed;

        GetTransition(ref startTransition, out st);
        GetTransition(ref endTransition, out ed);

        fadeImage.color = st.FadeImageColor;

        StartCoroutine(TransitionCoroutine(st, ed));
        StartCoroutine(FadeCoroutine(fadeStart, fadeEnd, st.Length, ed.Length, action));
    }
    public void Excute(string startTransition, string endTransition, Action action, float fadeStart, float fadeEnd, float duration)
    {
        ScreenTransitionData st, ed;

        GetTransition(ref startTransition, out st);
        GetTransition(ref endTransition, out ed);

        fadeImage.color = st.FadeImageColor;

        StartCoroutine(TransitionCoroutine(st, ed, duration));
        StartCoroutine(FadeCoroutine(fadeStart, fadeEnd, st.Length, ed.Length, action, duration));
    }
    IEnumerator TransitionCoroutine(ScreenTransitionData startTransition, ScreenTransitionData endTransition)
    {
        ParticlePlay(startTransition);

        yield return new WaitForSeconds(startTransition.Length);

        ParticlePlay(endTransition);
    }
    IEnumerator TransitionCoroutine(ScreenTransitionData startTransition, ScreenTransitionData endTransition, float duration)
    {
        ParticlePlay(startTransition);

        yield return new WaitForSeconds(duration + startTransition.Length);

        ParticlePlay(endTransition);
    }
    IEnumerator FadeCoroutine(float fadeStart, float fadeEnd, float startTransitionLength, float endTransitionLength, Action action)
    {
        float t = 0;
        float max = startTransitionLength;

        Color startColor = fadeImage.color;
        Color endColor = fadeImage.color;

        startColor.a = 0f;
        endColor.a = 1f;

        while (t < max)
        {
            t += Time.deltaTime;

            fadeImage.color = Color.Lerp(startColor, endColor, Mathf.InverseLerp(fadeStart, max, t));

            yield return null;
        }

        action.Invoke(); // 함수 실행

        t = 1f;

        max = endTransitionLength;

        while (t > 0)
        {
            t -= Time.deltaTime;

            fadeImage.color = Color.Lerp(startColor, endColor, Mathf.InverseLerp(fadeEnd, max, t));

            yield return null;
        }
    }
    IEnumerator FadeCoroutine(float fadeStart, float fadeEnd, float startTransitionLength, float endTransitionLength, Action action, float duration)
    {
        Color fadeColor = fadeImage.color;

        if (fadeStart > 0f) yield return new WaitForSeconds(fadeStart);

        float startFadeDuration = startTransitionLength - fadeStart;
        if (startFadeDuration <= 0f)
        {
            fadeColor.a = 1f;
            fadeImage.color = fadeColor;
        }
        else
        {
            float elapsed = 0f;
            while (elapsed < startFadeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / startFadeDuration);
                fadeColor.a = Mathf.Lerp(0f, 1f, progress);
                fadeImage.color = fadeColor;
                yield return null;
            }
        }

        action?.Invoke();

        if (duration > 0f) yield return new WaitForSeconds(duration);

        if (fadeEnd > 0f) yield return new WaitForSeconds(fadeEnd);

        float endFadeDuration = endTransitionLength - fadeEnd;
        if (endFadeDuration <= 0f)
        {
            fadeColor.a = 0f;
            fadeImage.color = fadeColor;
        }
        else
        {
            float elapsed = 0f;
            while (elapsed < endFadeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / endFadeDuration);
                fadeColor.a = Mathf.Lerp(1f, 0f, progress);
                fadeImage.color = fadeColor;
                yield return null;
            }
        }
    }
    IEnumerator FadeCoroutine(float fadeStart, float fadeEnd, float startTransitionLength, float endTransitionLength, string sceneName, ScreenTransitionData st, ScreenTransitionData ed)
    {
        float t = 0;
        float max = startTransitionLength;

        Color startColor = fadeImage.color;
        Color endColor = fadeImage.color;

        startColor.a = 0f;
        endColor.a = 1f;

        ParticlePlay(st);

        while (t < max)
        {
            t += Time.deltaTime;

            fadeImage.color = Color.Lerp(startColor, endColor, Mathf.InverseLerp(fadeStart, max, t));

            yield return null;
        }

        yield return WaitForSceneLoad(sceneName);

        ParticlePlay(ed);

        max = endTransitionLength;

        t = max;

        yield return null;

        while (t > 0)
        {
            t -= Time.deltaTime;

            fadeImage.color = Color.Lerp(startColor, endColor, Mathf.InverseLerp(fadeEnd, max, t));

            yield return null;
        }
    }
    private IEnumerator WaitForSceneLoad(string sceneName)
    {
        // 씬 로딩 시작
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // 씬 로딩이 완료될 때까지 대기
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        Debug.Log("Scene Loaded!");
    }
    private void GetTransition(ref string transitionName, out ScreenTransitionData data)
    {
        data = null;

        foreach (ScreenTransitionData transition in TransitionsData)
        {
            if (transition.Name.Equals(transitionName))
            {
                data = transition;

                transition.Material.SetColor("_Color", transition.TransitionColor);

                return;
            }
        }
        Debug.LogError("요청한 Transition을 찾지 못함 : " + transitionName);
    }
    private void ParticlePlay(ScreenTransitionData screenTransitionData)
    {
        if (!particleDict.ContainsKey(screenTransitionData.Name))
        {
            particleDict[screenTransitionData.Name] = Instantiate(screenTransitionData.Particle, canvasTransform);

            particleDict[screenTransitionData.Name].transform.localPosition = Vector3.zero;

            particleDict[screenTransitionData.Name].transform.SetAsFirstSibling();
        }

        particleDict[screenTransitionData.Name].Play();
    }
    public static Image GetFadeImage()
    {
        return instance.fadeImage;
    }
}
