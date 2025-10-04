using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class TitleScript : MonoBehaviour
{
    public bool isBgm = false;
    public GameObject controllerPanel;
    public GameObject optionPanel;
    private void Start()
    {
        if (isBgm) 
            SoundManager.Play("TitleBGM", SoundType.Background);

        Time.timeScale = 1;
    }

    public void GameSceneLoad()
    {
        FadeInOut.FadeStart(LoadScene);
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
    private void LoadScene()
    {
        SoundManager.Play("GameBGM", SoundType.Background);

        SceneManager.LoadScene("Game");
    }
    public void ExitButton()
    {/*
#if UNITY_WEBGL && !UNITY_EDITOR
    CloseTab(); // 웹 빌드 시에만 JavaScript 함수 호출
#else*/
    Application.Quit(); // 에디터나 다른 환경에서는 일반 종료
//#endif
    }

    public void Update()
    {
        // TODO : ESC 입력 변경 필요
        if (Input.GetKeyDown(KeyCode.Escape)){
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
}