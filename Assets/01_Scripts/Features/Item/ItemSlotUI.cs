using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public DescriptionLabel descriptionLabel;
    public Image itemSlotIcon;

    Item item = null;

    public bool SetItemUI(Item item)
    {
        if (this.item != null) return false;

        this.item = item;

        itemSlotIcon.sprite = item.icon;

        itemSlotIcon.enabled = true;

        descriptionLabel.SetLabel(item.Name, item.description, item.icon);

        return true;
    }

    private void OnDisable()
    {
        if (descriptionLabel.gameObject.activeSelf)
        {
            descriptionLabel.gameObject.SetActive(false);
        }
    }

    public void MouseEnter()
    {
        if (item != null)
        {
            descriptionLabel.SetLabel(item.Name, item.description, item.icon);

            descriptionLabel.gameObject.SetActive(true);
        }
    }
    void SetDescriptionLabel()
    {

    }
    public void MouseExit()
    {
        descriptionLabel.gameObject.SetActive(false);
    }
}
