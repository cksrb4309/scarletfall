using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    public static FadeInOut instance;
    public float fadeTime;
    public float fadeSpeed;
    public Image fadeImage;
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
    public static void FadeStart(Action action)
    {
        instance.TryFade(action);
    }
    public void TryFade(Action action)
    {
        StartCoroutine(FadeCoroutine(action));
    }
    private IEnumerator FadeCoroutine(Action action)
    {
        float fadeValue = 0;

        while (fadeValue < 1)
        {
            fadeValue += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(1, 1, 1, fadeValue);
            yield return null;
        }

        action();

        yield return new WaitForSeconds(fadeTime);

        while (fadeValue > 0)
        {
            fadeValue -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(1, 1, 1, fadeValue);
            yield return null;
        }
    }
}
