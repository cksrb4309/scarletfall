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
                ((DamageType)i).ToString(), damageTextPrefabs[i], 10, canvas);
        }
    }
    // damageType : 0은 플레이어가 주는 데미지,
    // 1은 몬스터가 주는 데미지,
    // 2는 플레이어 체력 회복,
    // 3은 플레이어의 크리티컬 데미지
    // 4는 플레이어의 극대화 데미지
    public static void SetDamage(float damage, Vector3 pos, DamageType damageType = DamageType.PlayerToMonsterNormal)
    {
        DamageText damageText = PoolingManager.Instance.GetObject(
            damageType.ToString()).GetComponent<DamageText>();

        pos.x += Random.Range(-instance.randomX, instance.randomX);
        pos.y += Random.Range(-instance.randomY, instance.randomY);
        pos.y += 0.3f;

        damageText.transform.position = pos;

        string str = (damage % 1 == 0) ? damage.ToString() : Mathf.Floor(damage).ToString();

        damageText.SetDamageText(str, damageType);
    }
    public static void Shield(Vector3 pos)
    {
        DamageText damageText = PoolingManager.Instance.GetObject(
            DamageType.PlayerShield.ToString()).GetComponent<DamageText>();

        damageText.transform.position = pos;

        damageText.SetDamageText(string.Empty, DamageType.PlayerShield);
    }
    public static void Avoid(Vector3 pos)
    {
        DamageText damageText = PoolingManager.Instance.GetObject(
            DamageType.PlayerAvoid.ToString()).GetComponent<DamageText>();

        damageText.transform.position = pos;

        damageText.SetDamageText(string.Empty, DamageType.PlayerAvoid);
    }
}


