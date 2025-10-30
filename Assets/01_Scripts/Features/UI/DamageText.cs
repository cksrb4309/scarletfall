using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TMP_Text damageTextUI;
    public DamageType damageType = DamageType.None;
    private string returnName = string.Empty;
    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Time.fixedDeltaTime * 0.5f);
    }
    void ReturnDamageText()
    {
        PoolingManager.Instance.ReturnObject(returnName, gameObject);
    }
    public void SetDamageText(string text, DamageType damageType)
    {
        if (this.damageType != damageType)
        {
            this.damageType = damageType;

            returnName = damageType.ToString();
        }
        if (text != string.Empty)
        {
            damageTextUI.text = text;
        }
    }
}
