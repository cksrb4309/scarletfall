using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMoveAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Vector3 beforePosition;
    [SerializeField] Vector3 afterPosition;
    [SerializeField] float duration = 0.5f;
    [SerializeField] Ease ease = Ease.Linear;
    Tween currentTween = null;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Move(afterPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Move(beforePosition);
    }
    private void Move(Vector3 position)
    {
        if (currentTween != null) currentTween.Kill();

        currentTween = transform.DOLocalMove(position, duration).SetEase(ease);
    }
}
