using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionLabel : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text description;
    public Image itemIcon;

    public void SetLabel(string itemName, string description, Sprite itemIcon)
    {
        this.itemName.text = itemName;
        this.description.text = description;
        this.itemIcon.sprite = itemIcon;
    }
}
