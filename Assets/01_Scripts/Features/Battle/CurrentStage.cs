using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CurrentStage : MonoBehaviour
{
    public BattleLoader loader;

    public GameObject panelGroup;

    public Transform[] positions;

    public Image[] allImage;

    public OnPlayerSprite onPlayerSprite;

    public Image playerImage;
    public Image panelImage;


    public Transform playerPos;

    int currentIndex = 0;

    Color baseColor;

    private void Start()
    {
        NextMove();

        baseColor = allImage[0].color;
    }

    public void NextMove()
    {
        StartCoroutine(NextMoveCoroutine());
    }
    IEnumerator NextMoveCoroutine()
    {
        panelGroup.SetActive(true);

        Color uiColor = baseColor;
        Color playerColor = Color.white;

        float t = 0;

        uiColor.a = t;
        playerColor.a = t;

        playerPos.position = positions[currentIndex].position;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;

            uiColor.a = t;
            playerColor.a = t;

            for (int i = 0; i < allImage.Length; i++)
                allImage[i].color = uiColor;
            playerImage.color = playerColor;
            panelImage.color = playerColor;

            yield return null;
        }

        playerPos.position = positions[currentIndex].position;

        onPlayerSprite.StartMove();


        t = 0;


        while (t < 1f)
        {
            Vector3 start = positions[currentIndex].position;
            Vector3 end = positions[currentIndex + 1].position;

            t += Time.deltaTime * 0.3f;

            playerPos.position = Vector3.Lerp(start, end, t);

            yield return null;
        }
        currentIndex++;

        onPlayerSprite.StopMove();
        
        yield return new WaitForSeconds(0.5f);

        t = 1f;

        while (t > 0)
        {
            t -= Time.deltaTime * 2f;

            uiColor.a = t;
            playerColor.a = t;

            for (int i = 0; i < allImage.Length; i++)
                allImage[i].color = uiColor;

            playerImage.color = playerColor;
            panelImage.color = playerColor;

            yield return null;
        }

        panelGroup.SetActive(false);
    }
}
