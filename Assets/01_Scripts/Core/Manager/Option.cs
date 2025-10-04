using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    public static Difficulty difficulty = Difficulty.Normal;

    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;

    public TMP_Text difficultyText;

    public Color[] color;
    private void Start()
    {


        bgmVolumeSlider.value = SoundManager.GetBGMVolume();
        sfxVolumeSlider.value = SoundManager.GetSFXVolume();

        ChangeDifficulty(difficulty);
    }
    void ChangeDifficulty(Difficulty difficulty)
    {
        Option.difficulty = difficulty;

        switch (difficulty)
        {
            case Difficulty.Easy:
                difficultyText.text = "Easy";
                difficultyText.color = color[0];
                break;
            case Difficulty.Normal:
                difficultyText.text = "Normal";
                difficultyText.color = color[1];
                break;
            case Difficulty.Hard:
                difficultyText.text = "Hard";
                difficultyText.color = color[2];
                break;
        }
    }
    public void ExitButton()
    {
        gameObject.SetActive(false);

        Time.timeScale = 1.0f;
    }
    public void EasySelect()
    {
        ChangeDifficulty(Difficulty.Easy);
    }
    public void NormalSelect()
    {
        ChangeDifficulty(Difficulty.Normal);
    }
    public void HardSelect()
    {
        ChangeDifficulty(Difficulty.Hard);
    }
    public void BGM_Volume_Change()
    {
        SoundManager.SetBGMVolume(bgmVolumeSlider.value);
    }
    public void SFX_Volume_Change()
    {
        SoundManager.SetSFXVolume(sfxVolumeSlider.value);
    }
}
public enum Difficulty
{
    Easy = 0,
    Normal = 1,
    Hard = 2
}