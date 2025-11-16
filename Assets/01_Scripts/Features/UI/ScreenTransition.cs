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

    private ScreenTransitionData startTransitionData = null;
    private ScreenTransitionData endTransitionData = null;

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

    public static Image GetFadeImage()
    {
        return instance.fadeImage;
    }
    public static void Play(ScreenTransitionOptions options)
    {
        instance.ExecuteTransition(options);
    }
    private void ExecuteTransition(ScreenTransitionOptions options)
    {
        SetTransitionData(options);

        StartCoroutine(RunTransitionRoutine(options));
    }
    IEnumerator RunTransitionRoutine(ScreenTransitionOptions options)
    {
        // 1) 화면전환 시작 효과
        ParticlePlay(startTransitionData);

        // 2) 페이드 인 시작 전 딜레이
        if (options.FadeStart > 0f) yield return new WaitForSeconds(options.FadeStart);

        // 3) 페이드 인
        float fadeInDuration = startTransitionData.Length - options.FadeStart;
        Color baseColor = startTransitionData.FadeImageColor;
        baseColor.a = 0f;
        fadeImage.color = baseColor;

        yield return FadeAlpha(0f, 1f, fadeInDuration);

        // 4) 완료 콜백 또는 씬 로드
        if (options.OnTransitionComplete != null)
        {
            options.OnTransitionComplete.Invoke();
        }
        if (!string.IsNullOrEmpty(options.SceneName))
        {
            yield return WaitForSceneLoad(options.SceneName);
        }

        // 5) Fade 유지 시간
        if (options.FadeDuration > 0f) yield return new WaitForSeconds(options.FadeDuration);

        // 6) 화면전환 종료 효과
        ParticlePlay(endTransitionData);

        // 7) 페이드 아웃 시작 전 딜레이
        if (options.FadeEnd > 0f) yield return new WaitForSeconds(options.FadeEnd);

        // 8) 페이드 아웃
        {
            float fadeOutDuration = endTransitionData.Length - options.FadeEnd;
            yield return FadeAlpha(1f, 0f, fadeOutDuration);
        }
    }
    IEnumerator FadeAlpha(float from, float to, float duration)
    {
        Color color = fadeImage.color;
        color.a = from;
        fadeImage.color = color;

        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            color.a = Mathf.Lerp(from, to, normalized);
            fadeImage.color = color;

            yield return null;
        }

        color.a = to;
        fadeImage.color = color;
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
    }
    private void SetTransitionData(ScreenTransitionOptions options)
    {
        if (options.StartTransitionData != null && options.EndTransitionData != null)
        {
            SetTransitionData(options.StartTransitionData, options.EndTransitionData);
        }
        else
        {
            SetTransitionData(options.StartTransitionName, options.EndTransitionName);
        }
    }
    private void SetTransitionData(string st, string ed)
    {
        foreach (ScreenTransitionData transition in TransitionsData)
        {
            if (transition.Name.Equals(st))
            {
                startTransitionData = transition;

                transition.Material.SetColor("_Color", transition.TransitionColor);
            }
            if (transition.Name.Equals(ed))
            {
                endTransitionData = transition;

                transition.Material.SetColor("_Color", transition.TransitionColor);
            }
        }
    }
    private void SetTransitionData(ScreenTransitionData st, ScreenTransitionData ed)
    {
        startTransitionData = st;
        endTransitionData = ed;

        st.Material.SetColor("_Color", st.TransitionColor);
        ed.Material.SetColor("_Color", ed.TransitionColor);
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
}

public class ScreenTransitionOptions
{
    public float FadeStart = 0f;
    public float FadeEnd = 1f;
    public float FadeDuration = 0.5f;
    public string SceneName = null;
    public string StartTransitionName = null;
    public string EndTransitionName = null;
    public Action OnTransitionComplete = null;
    public ScreenTransitionData StartTransitionData = null;
    public ScreenTransitionData EndTransitionData = null;
    public ScreenTransitionOptions() { }
}
