using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MagicProjectile : MonoBehaviour
{
    public float speed = 7f;

    public float destroyDelay = 4f;

    Vector3 dir;

    List<Monster> MonsterPos { get { return Battle.instance.monsterPos; } }

    private void OnEnable()
    {
        StartCoroutine(ReturnCoroutine());
    }
    public void Setting()
    {
        if (Battle.instance.monsterPos.Count > 0)
        {
            dir = MonsterPos[Random.Range(0, MonsterPos.Count)].
                GetCenterPosition() - transform.position;

            dir.Normalize();
        }
        else
        {
            dir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
            dir.Normalize();
        }
    }
    private void FixedUpdate()
    {
        transform.position += dir * Time.fixedDeltaTime * speed;
    }
    private IEnumerator ReturnCoroutine()
    {
        yield return new WaitForSeconds(destroyDelay);

        PoolingManager.Instance.ReturnObject("MagicProjectile", gameObject);
    }
}
