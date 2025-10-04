using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleLoader : MonoBehaviour
{
    public AnimationCurve speedCurve;
    public Collider2D cd;
    public SpriteRenderer blinkSr;
    public GameObject portal;
    public GameObject lobby;
    public float max = 0.8f;
    public float min = 0.6f;
    public float alphaSpeed;

    public TMP_Text stageTextUI;
    public RectTransform left;
    public RectTransform right;
    public Image leftImage;
    public Image rightImage;
    public Sprite redLeft;
    public Sprite redRight;
    public Color redTextColor;
    public Image[] parallaxBackgrounds;
    public Sprite[] redBackgrounds;
    public GameObject defaultGround;
    public GameObject redHoodBattleGround;
    public SpriteRenderer outlineSprite;
    public RectTransform centerRect;

    Vector2 pos;

    int stage = -1;
    bool isStageEntered = false;

    float Alpha
    {
        get { return alpha; }
        set { alpha = value; blinkSr.color = new Color(1, 1, 1, alpha); }
    }
    float alpha;
    float baseSizeY;
    bool isCheck = false;
    bool dir = false;
    // dir는 값의 방향으로 true일 때는 증가 false일 때는 알파값을 감소시킴


    public void BattleLoad()
    {
        pos.Set(Screen.width * 1.6f, 0);

        centerRect.anchoredPosition = pos;

        SoundManager.Play("StartStage", SoundType.Effect);

        if (++stage == 10)
        {
            stageTextUI.text = "빨간 망토";

            leftImage.sprite = redLeft;
            rightImage.sprite = redRight;

            stageTextUI.color = redTextColor;

            SoundManager.Play("BossBGM", SoundType.Background);
        }
        else
            stageTextUI.text = "Stage " + (stage+1).ToString();

        StartCoroutine(LoadCoroutine(HideLobby, Battle.instance.StartBattle));
    }
    public void ClearLoad()
    {
        pos.Set(Screen.width * 1.6f, 0);

        centerRect.anchoredPosition = pos;

        stageTextUI.text = "Clear";

        if (stage != 10)
        {
            StartCoroutine(
                LoadCoroutine(ShowLobby,
                Inventory.instance.selectPanelGroup.StartSelectItem));
        }
        dir = false;
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

    IEnumerator LoadCoroutine(Action mid = null, Action end = null)
    {
        float t = 0;

        Vector2 startPos = pos;

        Vector2 midPos = Vector2.zero;

        Vector2 endPos = -pos;

        leftImage.enabled = true;
        rightImage.enabled = true;

        while (t < 5f)
        {
            t += Time.deltaTime * speedCurve.Evaluate(t/5);
            
            centerRect.anchoredPosition = Vector2.Lerp(startPos, midPos, t / 5f);

            yield return null;
        }

        centerRect.anchoredPosition = midPos;

        if (mid != null) mid.Invoke();

        if (stage == 10)
        {
            for (int i = 0; i < parallaxBackgrounds.Length; i++)
                parallaxBackgrounds[i].sprite = redBackgrounds[i];

            defaultGround.SetActive(false);
            redHoodBattleGround.SetActive(true);
        }


        t = 0;

        startPos = midPos;

        while (t < 5f)
        {
            t += Time.deltaTime * speedCurve.Evaluate(t/5);

            centerRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t / 5);

            yield return null;
        }

        centerRect.anchoredPosition = endPos;

        leftImage.enabled = false;
        rightImage.enabled = false;

        if (end != null) end.Invoke();
    }
    // TODO : 상호작용 Input 변경 필요
    private void Update()
    {
        if (!isStageEntered && isCheck)
        {
            if (Input.GetKeyDown(KeyCode.F))
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
            if (dir)
            {
                if (Alpha < max) Alpha += Time.fixedDeltaTime * alphaSpeed;
                else dir = false;
            }
            else
            {
                if (Alpha > min) Alpha -= Time.fixedDeltaTime * alphaSpeed;
                else dir = true;
            }
        }
        else if (Alpha > 0) 
        {
            Alpha -= Time.fixedDeltaTime * alphaSpeed;
        }
        else
        {
            outlineSprite.enabled = false;
        }
    }
    public void StartAnimation()
    {
        Alpha = 0;
        dir = true;
    }
    public void EndAnimation()
    {
        isCheck = false;
        dir = false;
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
}
