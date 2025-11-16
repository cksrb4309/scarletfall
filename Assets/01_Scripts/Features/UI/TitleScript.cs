using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;

public class TitleScript : MonoBehaviour
{
    public bool isBgm = false;
    public GameObject controllerPanel;
    public GameObject optionPanel;

    InputActionReference escapeInputAction = null;

    private void Start()
    {
        if (isBgm)
        {
            SoundManager.Play("TitleBGM", SoundType.Background);
        }

        Time.timeScale = 1;
    }

    public void GameSceneLoad()
    {
        ScreenTransition.Play(new ScreenTransitionOptions
        {
            StartTransitionName = "Leaf_FadeOut",
            EndTransitionName = "Leaf_FadeIn",
            OnTransitionComplete = () =>
            {
                SoundManager.Play("GameBGM", SoundType.Background);
            },
            SceneName = "Game",
            FadeStart = 0f,
            FadeEnd = 0f,
            FadeDuration = 0.5f
        });
    }
    public void OpenControlPanel()
    {
        controllerPanel.SetActive(true);
    }
    public void OpenOptionPanel()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void CloseControlPanel()
    {
        controllerPanel.SetActive(false);
    }
    public void ExitButton()
    {
        Application.Quit(); // 에디터나 다른 환경에서는 일반 종료
    }
    public void Update()
    {
        if (escapeInputAction.action.WasPressedThisFrame())
        {
            if (!optionPanel.activeSelf)
            {
                OpenOptionPanel();
            }
            else
            {
                CloseOptionPanel();
            }
        }
    }
    private void OnEnable()
    {
        escapeInputAction = InputManager.GetInputAction(InputType.Escape);
    }
    private void OnDisable()
    {
        InputManager.Release(InputType.Escape);
    }
}