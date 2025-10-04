using UnityEngine;
using UnityEngine.UI;

public class OnEnbleCopyImage : MonoBehaviour
{
    public Image copyImage;
    public Image myImage;
    private void OnEnable()
    {
        myImage.sprite = copyImage.sprite;

        if (myImage.sprite == null) myImage.enabled = false;
        else myImage.enabled = true;
    }
}
