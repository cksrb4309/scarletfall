using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class DOTweenHelper
{
    public static void Fade(Image image, float duration, float waitTime)
    {
        image.DOFade(1f, duration).OnComplete(() => { image.DOFade(0f, duration).SetDelay(waitTime); });
    }
    public static void MoveUpAnimation(Transform transform, float startY, float endY, float duration)
    {
        transform.localPosition = Vector3.up * startY;

        transform.DOLocalMoveY(endY, duration);
    }
}
