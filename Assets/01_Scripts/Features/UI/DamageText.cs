using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Time.fixedDeltaTime * 0.5f);
    }
    void ReturnDamageText()
    {
        PoolingManager.Instance.ReturnObject("DamageText", gameObject);
    }
}
