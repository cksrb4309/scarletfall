using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleLoader : MonoBehaviour
{
    public Collider2D cd;
    public SpriteRenderer blinkSr;
    public GameObject portal;
    public GameObject lobby;
    public float max = 0.8f;
    public float min = 0.6f;
    public float alphaSpeed;

    public TMP_Text stageTextUI;
    public CanvasGroup stageTextCanvasGroup;

    public Color redTextColor;
    public Image[] parallaxBackgrounds;
    public Sprite[] redBackgrounds;
    public GameObject defaultGround;
    public GameObject redHoodBattleGround;
    public SpriteRenderer outlineSprite;

    int stage = -1;
    bool isStageEntered = false;

    float BlinkImageAlpha
    {
        get => blinkImageAlpha;
        set
        {
            blinkImageAlpha = value;
            blinkSr.color = new Color(1, 1, 1, blinkImageAlpha);
        }
    }
    float blinkImageAlpha;

    private Image fadeImage;

    private InputActionReference interactInputAction = null;

    bool isCheck = false;
    bool blinkAlphaDir = false;
    // dir는 값의 방향으로 true일 때는 증가 false일 때는 알파값을 감소시킴


    public void BattleLoad()
    {
        SoundManager.Play("StartStage", SoundType.Effect);

        if (++stage == 10)
        {
            stageTextUI.text = "빨간 망토";

            stageTextUI.color = redTextColor;

            SoundManager.Play("BossBGM", SoundType.Background);
        }
        else
        {
            stageTextUI.text = "Stage " + (stage + 1).ToString();
        }
        ScreenTransition.Play(new ScreenTransitionOptions
        {
            StartTransitionName = stage < 10 ? "Leaf_FadeOut" : "RedLeaf_FadeOut",
            EndTransitionName = stage < 10 ? "Leaf_FadeIn" : "RedLeaf_FadeIn",
            FadeStart = 0f,
            FadeEnd = 0f,
            FadeDuration = 2f,
            OnTransitionComplete = () =>
            {
                HideLobby();
                SetRedHoodLevel();
                DOVirtual.DelayedCall(5f, () => Battle.instance.StartBattle());
            }
        });
    }
    public void ClearLoad()
    {
        stageTextUI.text = "Clear";
        blinkAlphaDir = false;

        if (stage != 10)
        {
            ScreenTransition.Play(new ScreenTransitionOptions
            {
                StartTransitionName = "Leaf_FadeOut",
                EndTransitionName = "Leaf_FadeIn",
                FadeStart = 0f,
                FadeEnd = 0f,
                FadeDuration = 2f,
                OnTransitionComplete = () =>
                {
                    ShowLobby();
                    DOVirtual.DelayedCall(5f, () => Inventory.instance.selectPanelGroup.StartSelectItem());
                }
            });
        }
    }
    public void ActivePortal()
    {
        isStageEntered = false;

        cd.enabled = true;
    }
    void ShowLobby()
    {
        lobby.SetActive(true);

        portal.SetActive(true);
    }
    void HideLobby()
    {
        lobby.SetActive(false);

        portal.SetActive(false);

        isStageEntered = true;

        isCheck = false;
    }

    // 스테이지가 10인 경우에만 설정하는 레벨
    private void SetRedHoodLevel()
    {
        if (stage != 10)
        {
            return;
        }

        for (int i = 0; i < parallaxBackgrounds.Length; i++)
        {
            parallaxBackgrounds[i].sprite = redBackgrounds[i];
        }

        defaultGround.SetActive(false);
        redHoodBattleGround.SetActive(true);
    }
    
    public void StartAnimation()
    {
        BlinkImageAlpha = 0;
        blinkAlphaDir = true;
    }
    public void EndAnimation()
    {
        isCheck = false;
        blinkAlphaDir = false;
    }

    public void DisableStageText()
    {
        stageTextUI.text = string.Empty;
        stageTextUI.enabled = false;
    }

    #region 유니티 콜백 함수

    private void Update()
    {
        if (!isStageEntered && isCheck)
        {
            if (interactInputAction.action.WasPressedThisFrame())
            {
                isStageEntered = true;

                isCheck = false;

                cd.enabled = false;

                BattleLoad();
            }
        }
    }
    private void FixedUpdate()
    {
        if (!isStageEntered && isCheck)
        {
            if (blinkAlphaDir)
            {
                if (BlinkImageAlpha < max) BlinkImageAlpha += Time.fixedDeltaTime * alphaSpeed;
                else blinkAlphaDir = false;
            }
            else
            {
                if (BlinkImageAlpha > min) BlinkImageAlpha -= Time.fixedDeltaTime * alphaSpeed;
                else blinkAlphaDir = true;
            }
        }
        else if (BlinkImageAlpha > 0)
        {
            BlinkImageAlpha -= Time.fixedDeltaTime * alphaSpeed;
        }
        else
        {
            outlineSprite.enabled = false;
        }
        stageTextCanvasGroup.alpha = fadeImage.color.a;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCheck = true;

            outlineSprite.enabled = true;

            StartAnimation();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCheck = false;

            outlineSprite.enabled = false;

            EndAnimation();
        }
    }
    private void OnEnable()
    {
        interactInputAction = InputManager.GetInputAction(InputType.Interact);
    }
    private void OnDisable()
    {
        InputManager.Release(InputType.Interact);
    }
    private void Start()
    {
        fadeImage = ScreenTransition.GetFadeImage();
    }
    #endregion
}
