using System.Collections;
using UnityEngine;

public class FastDownEffect : MonoBehaviour
{
    public Collider2D cd;

    public int count; // 생성 개수

    public float width; // 생성 너비

    public float height; // 최대 높이

    public void Enable(Vector3 pos)
    {
        transform.position = pos;

        Vector3 randomPos = Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(-width, width);
            float y = Random.Range(0, height);

            GameObject obj = PoolingManager.Instance.GetObject("FastDownAttackEffectParts");

            obj.transform.parent = transform;

            randomPos.Set(x, y, 0);

            obj.transform.localPosition = randomPos;
        }
        StartCoroutine(AttackColliderCoroutine());
    }

    IEnumerator AttackColliderCoroutine()
    {
        cd.enabled = true;

        yield return new WaitForSeconds(0.1f);

        cd.enabled = false;
    }
}
