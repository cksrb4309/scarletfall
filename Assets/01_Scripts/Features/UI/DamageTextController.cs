using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DamageTextController : MonoBehaviour
{
    static DamageTextController instance = null;
    [SerializeField] private Transform canvas;
    [SerializeField] private List<GameObject> damageTextPrefabs;
    [SerializeField] float randomX;
    [SerializeField] float randomY;
    
    private void Start()
    {
        instance = this;
        for (int i = 0; i < damageTextPrefabs.Count; i++)
        {
            PoolingManager.Instance.CreatePool(
                i.ToString() + "DamageText", damageTextPrefabs[i], 10, canvas);
        }
    }
    // damageType : 0은 플레이어가 주는 데미지,
    // 1은 몬스터가 주는 데미지,
    // 2는 플레이어 체력 회복,
    // 3은 플레이어의 크리티컬 데미지
    // 4는 플레이어의 극대화 데미지
    public static void SetDamage(float damage, Vector3 pos, int damageType = 0)
    {
        TMP_Text textUI = PoolingManager.Instance.GetObject(
            damageType.ToString() + "DamageText").GetComponentInChildren<TMP_Text>();

        pos.x += Random.Range(-instance.randomX, instance.randomX);
        pos.y += Random.Range(-instance.randomY, instance.randomY);

        pos.y += 0.3f;

        textUI.transform.parent.position = pos;

        string str = (damage % 1 == 0) ? damage.ToString() : Mathf.Floor(damage).ToString();

        textUI.text = str;
    }
    public static void Shield(Vector3 pos)
    {
        TMP_Text textUI = PoolingManager.Instance.GetObject(
            "5DamageText").GetComponentInChildren<TMP_Text>();

        textUI.transform.parent.position = pos;
    }
    public static void Avoid(Vector3 pos)
    {
        TMP_Text textUI = PoolingManager.Instance.GetObject(
            "6DamageText").GetComponentInChildren<TMP_Text>();

        textUI.transform.parent.position = pos;
    }
}


