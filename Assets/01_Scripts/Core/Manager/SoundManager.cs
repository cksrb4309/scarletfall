using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance = null;

    public AudioSource[] backgroundAudios;
    public AudioSource effectAudio;

    public float bgmVolume = 1.0f;     // 브금 최대 소리 크기
    public float sfxVolume = 1.0f;     // 효과음 최대 소리 크기


    public Sound[] soundArray;

    public AudioClip[] footStapSounds;
    public AudioClip[] monsterDieSounds;

    Dictionary<string, AudioClip> clipDictionary = new Dictionary<string, AudioClip>();

    bool isBackgroundSoundPlaying = false;
    int currentBackgroundIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            for (int i = 0; i < soundArray.Length; i++)
                clipDictionary.Add(soundArray[i].clipName, soundArray[i].audioClip);

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static void Play(string clipName, SoundType type)
    {
        Debug.Log("clipName : " + clipName.ToString());
        if (type == SoundType.Effect)
        {
            instance.EffectPlay(clipName);
        }
        else
        {
            instance.BackgroundPlay(clipName);
        }
    }
    public static void SetBGMVolume(float volume)
    {
        instance.SetBGM(volume);
    }
    void SetBGM(float volume)
    {
        backgroundAudios[0].volume = volume;
        backgroundAudios[1].volume = volume;

        bgmVolume = volume;
    }
    public static void SetSFXVolume(float volume)
    {
        instance.SetSFX(volume);
    }
    void SetSFX(float volume)
    {
        effectAudio.volume = volume;

        sfxVolume = volume;
    }
    void EffectPlay(string clipName)
    {
        if (clipName == string.Empty)
        {
            Debug.Log("Audio가 비어있습니다");
        }
        else if (clipDictionary.ContainsKey(clipName))
        {
            Debug.Log("clipName:" + clipName.ToString());
            effectAudio.PlayOneShot(clipDictionary[clipName]);
        }
        else
        {
            Debug.LogWarning("clipName:" + clipName + "과(와) 일치하는 Sound가 없습니다");
        }
    }
    void BackgroundPlay(string clipName)
    {
        if (clipName == string.Empty)
        {
            Debug.Log("Audio가 비어있습니다");
        }
        else if (clipDictionary.ContainsKey(clipName))
        {
            if (isBackgroundSoundPlaying)
            {
                currentBackgroundIndex ^= 1;
                backgroundAudios[currentBackgroundIndex].clip = clipDictionary[clipName];

                StartCoroutine(BackgroundAudioFadeCoroutine());
            }
            else
            {
                isBackgroundSoundPlaying = true;

                backgroundAudios[currentBackgroundIndex].clip = clipDictionary[clipName];
                backgroundAudios[currentBackgroundIndex].Play();
            }
        }
        else
        {
            Debug.LogWarning("clipName:" + clipName + "과(와) 일치하는 Sound가 없습니다");
        }
    }
    IEnumerator BackgroundAudioFadeCoroutine()
    {
        float t = 0f;

        backgroundAudios[currentBackgroundIndex].Play();

        while (t < 1f)
        {
            t += Time.deltaTime * 0.3f;

            backgroundAudios[currentBackgroundIndex ^ 1].volume = Mathf.Lerp(bgmVolume, 0, t);
            backgroundAudios[currentBackgroundIndex].volume = Mathf.Lerp(0, bgmVolume, t);

            yield return null;
        }
        backgroundAudios[currentBackgroundIndex ^ 1].Stop();
    }
    public static void FootStapPlay()
    {
        instance.FootStap();
    }
    public void FootStap()
    {
        effectAudio.PlayOneShot(footStapSounds[Random.Range(0, footStapSounds.Length)]);
    }
    public static void MonsterDiePlay()
    {
        instance.MonsterDie();
    }
    public void MonsterDie()
    {
        effectAudio.PlayOneShot(monsterDieSounds[Random.Range(0, monsterDieSounds.Length)]);
    }
    public static float GetBGMVolume()
    {
        return instance.bgmVolume;
    }
    public static float GetSFXVolume()
    {
        return instance.sfxVolume;
    }
}

[System.Serializable]
public struct Sound
{
    public string clipName;
    public AudioClip audioClip;
}
public enum SoundType
{
    Background,
    Effect
}