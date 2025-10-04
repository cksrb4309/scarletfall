using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingProjectile : MonoBehaviour
{
    public float speed = 7f;

    public float destroyDelay = 5f;

    Monster target = null;

    Vector3 dir = Vector3.up;
    List<Monster> MonsterPos { get { return Battle.instance.monsterPos; } }
    private void OnEnable()
    {
        StartCoroutine(ReturnCoroutine());
    }
    private void FixedUpdate()
    {
        if (target == null)
        {
            Debug.Log("target Null");
            if (MonsterPos.Count > 0) 
            {
                target = MonsterPos[Random.Range(0, MonsterPos.Count)];
            }
            else
            {
                dir *= 0.1f;
            }
        }
        if (target != null)
        {
            while (target.gameObject.activeSelf == false)
            {
                if (MonsterPos.Count > 0)
                {
                    target = MonsterPos[Random.Range(0, MonsterPos.Count)];
                }
                else
                {
                    PoolingManager.Instance.ReturnObject("TrackingProjectile", gameObject);

                    break;
                }
            }

            dir = target.GetCenterPosition() - transform.position;
            dir.Normalize();
        }

        transform.position += dir * Time.fixedDeltaTime * speed;
    }

    private IEnumerator ReturnCoroutine()
    {
        yield return new WaitForSeconds(destroyDelay);

        PoolingManager.Instance.ReturnObject("TrackingProjectile", gameObject);
    }
}
