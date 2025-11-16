using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearPanel : MonoBehaviour
{
    public BattleLoader battleLoader;
    public TMP_Text difficultyText;
    public GameObject showItemPanel;
    public Color[] colors;
    private void OnEnable()
    {
        battleLoader.DisableStageText();

        if (difficultyText == null) return;

        switch (Option.difficulty)
        {
            case Difficulty.Easy:
                difficultyText.text = "Easy";
                difficultyText.color = colors[0];
                break;
            case Difficulty.Normal:
                difficultyText.text = "Normal";
                difficultyText.color = colors[1];
                break;
            case Difficulty.Hard:
                difficultyText.text = "Hard";
                difficultyText.color = colors[2];
                break;
        }
    }
    public void ShowItem()
    {
        showItemPanel.SetActive(!showItemPanel.activeSelf);
    }
    public void LoadScene()
    {
        Time.timeScale = 1;

        ScreenTransition.Play(new ScreenTransitionOptions
        {
            StartTransitionName = "Leaf_FadeOut",
            EndTransitionName = "Leaf_FadeIn",
            SceneName = "Title",
            FadeStart = 0f,
            FadeEnd = 0f,
            FadeDuration = 2f
        });
    }
}
