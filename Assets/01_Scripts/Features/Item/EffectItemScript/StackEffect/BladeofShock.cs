using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BladeofShock : StackEffect
{
    public float minDistance;

    float attackDelay = 0.1f;

    public ParticleSystem[] particles;

    List<Monster> MonsterList { get { return Battle.instance.monsterPos; } }

    List<Monster> attackList = new List<Monster>();

    Monster lastMob = null;

    public override void Play()
    {
        lastMob = null;

        lastMob = GameManager.Instance.lastHitMonster;

        attackList.Clear();

        attackList.Add(lastMob);

        lastMob.Hit(1200, false);

        SoundManager.Play("StaticAttack", SoundType.Effect);

        particles[0].transform.position = lastMob.GetCenterPosition();
        particles[0].Play();

        LightningMaterial.StartTextureChange(); // 애니메이션 실행

        StartCoroutine(SearchMonsterCoroutine(2));
    }

    IEnumerator SearchMonsterCoroutine(int n)
    {
        yield return new WaitForSeconds(attackDelay);

        SearchMonster(n);
    }

    void SearchMonster(int n)
    {
        if (n == 0) return;

        Monster mob = null;

        Vector2 targetPos = lastMob.transform.position;

        float minValue = minDistance;

        for (int i = 0; i < MonsterList.Count; i++)
        {
            bool isContains = false;
            for (int j = 0; j < attackList.Count; j++)
            {
                if (attackList[j].id == MonsterList[i].id){
                    isContains = true; break;
                }
            }

            if (!isContains)
            {
                Vector2 vec = targetPos - (Vector2)MonsterList[i].transform.position;

                float distance = Mathf.Abs(vec.x) + Mathf.Abs(vec.y);

                if (minDistance > distance)
                {
                    if (minValue > distance)
                    {
                        minValue = distance;

                        mob = MonsterList[i];
                    }
                }
            }
        }
        if (mob != null)
        {
            attackList.Add(mob);

            mob.Hit(1200, false);

            LightningGroup lg = PoolingManager.Instance.GetObject<LightningGroup>("LightningGroup");

            lg.Set(lastMob.GetCenterPosition(), mob.GetCenterPosition());

            particles[particles.Length - n].transform.position = mob.GetCenterPosition();
            particles[particles.Length - n].Play();

            lastMob = mob;

            StartCoroutine(SearchMonsterCoroutine(n-1));
        }
    }
}
