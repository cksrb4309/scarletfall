using System.Collections;
using UnityEngine;

public class FlyingEye : Monster
{
    public Animator animator;

    public Transform playerTransform;

    public float nad_1;
    public float nad_2;
    public float nad_3;
    public float nad_4;

    public float safeDistance_1;
    public float safeDistance_2;

    public float minHeight;
    public float maxHeight;

    public float randomOffsetX;
    public override void StartMonster()
    {
        base.StartMonster();

        playerTransform = GameManager.Instance.player.transform;

        current = -1;

        StartCoroutine(BehaviorCoroutine());
    }
    IEnumerator BehaviorCoroutine()
    {
        while (IsAlive)
        {
            yield return new WaitForSeconds((current == -1 || current == 3 ? nad_1 : current == 1 ? nad_2 : nad_3));


            if (IsAlive) StartTracking();
        }
    }

    int current = -1;
    // current가 0일 때는 플레이어와 거리를 벌림
    // current가 1일 때는 플레이어와 살짝 거리를 좁힘
    // current가 1일 때는 입을 천천히 벌림
    // current가 2일 때는 돌진함

    void StartTracking()
    {
        current++; // 다음 상태로 전환

        int dir = GetDir();

        transform.localScale = dir == 1 ? Vector3.one : new Vector3(-1, 1, 1);

        if (current == 4) current = 0;

        if (current == 0)
        {
            animator.SetTrigger("Idle");
            float x = playerTransform.position.x - (dir * safeDistance_1) + Random.Range(-randomOffsetX, randomOffsetX);
            float y = Mathf.Lerp(minHeight, maxHeight, Random.value);

            Vector3 pos = new Vector3(x, y, 0);

            StartCoroutine(LerpMove(pos));
        }
        else if (current == 1)
        {
            animator.SetTrigger("Idle");
            float x = playerTransform.position.x - (dir * safeDistance_2) + Random.Range(-randomOffsetX, randomOffsetX);
            float y = Mathf.Lerp(minHeight, maxHeight, Random.value);

            Vector3 pos = new Vector3(x, y, 0);

            StartCoroutine(LerpMove(pos));
        }
        else if (current == 2)
        {
            animator.SetTrigger("WaitAttack");
        }
        else if (current == 3)
        {
            animator.SetTrigger("Attack");

            StartCoroutine(AttackCoroutine());
        }
    }
    IEnumerator AttackCoroutine()
    {
        SoundManager.Play("FlyingEyeAttack", SoundType.Effect);

        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + (playerTransform.position - startPos).normalized * 10f;

        double t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, (float)t);
            yield return null;
        }
        transform.position = endPos;
    }
    IEnumerator LerpMove(Vector3 pos)
    {
        double t = 0;
        Vector3 startPos = transform.position;
        while (t < 1f)
        {
            t += Time.deltaTime / 2f;

            transform.position = Vector3.Lerp(startPos, pos, (float)t);

            yield return null;
        }

        transform.position = pos;
    }

    int GetDir()
    {
        return transform.position.x <= playerTransform.position.x ? 1 : -1;
    }
    public override void Hit(float damage, bool isMainAttack = true)
    {
        base.Hit(damage, isMainAttack);

        if (!IsAlive) Dead();
        //else animator.SetTrigger("Hit");
    }
    private void Dead()
    {
        animator.SetTrigger("Die1");

        GetComponent<Collider2D>().enabled = false;

        SoundManager.MonsterDiePlay();

        StopAllCoroutines();

        StartCoroutine(DeadCoroutine());
    }
    IEnumerator DeadCoroutine()
    {
        while (transform.position.y > -3f)
        {
            transform.Translate(Vector3.down * Time.deltaTime * 3f);

            yield return null;
        }
        transform.position = transform.position + new Vector3(0, -3f - transform.position.y, 0);

        SoundManager.Play("FlyingEyeLand", SoundType.Effect);

        animator.SetTrigger("Die2");
    }
    public void CompleteDead()
    {
        PoolingManager.Instance.ReturnObject("FlyingEye", gameObject);
    }
}