using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectPanel : MonoBehaviour
{
    public GameObject selectedObj;
    public Item item;
    public Image itemIcon;
    public TMP_Text nameTextUI;
    public TMP_Text descriptionTextUI;
    public TMP_Text itemGradeTextUI;

    public void SetSelectPanel(Item item)
    {
        this.item = item;
        itemIcon.sprite = item.icon;
        nameTextUI.text = item.Name;
        descriptionTextUI.text = item.description;
        itemGradeTextUI.color = item.gradeColor;
        itemGradeTextUI.text = item.itemGrade.ToString();
    }
    public void OnSelected()
    {
        selectedObj.SetActive(true);
    }
    public void OnUnSelected()
    {
        selectedObj.SetActive(false);
    }
}