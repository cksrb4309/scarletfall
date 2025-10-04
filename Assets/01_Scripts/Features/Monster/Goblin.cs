using UnityEngine;

public class Goblin : Monster
{
    public Animator ar;

    Transform playerTransform;

    bool isAttacking = false;

    public Collider2D searchCollider;

    public override void StartMonster()
    {
        base.StartMonster();

        playerTransform = GameManager.Instance.player.transform;

        searchCollider.enabled = false;
    }
    public void Run()
    {
        isAttacking = false;

        searchCollider.enabled = true;

        ar.SetTrigger("Run");
    }

    public void Attack()
    {
        isAttacking = true;

        searchCollider.enabled = false;

        ar.SetTrigger("Attack");
    }
    public void AttackSoundPlay()
    {
        SoundManager.Play("GoblinAttack", SoundType.Effect);
    }
    public override void Hit(float damage, bool isMainAttack = true)
    {
        base.Hit(damage, isMainAttack);

        if (IsAlive)
        {
            ar.SetTrigger("Hit");
        }
        else
        {
            ar.SetBool("Die", true);

            cd.enabled = false;

            SoundManager.MonsterDiePlay();
        }
    }
    void ReturnGoblin()
    {
        PoolingManager.Instance.ReturnObject("Goblin", gameObject);
    }
    private void FixedUpdate()
    {
        if (IsAlive)
        {
            if (!isAttacking)
            {
                if (hpBarParent.activeSelf)
                {
                    if (transform.position.x > playerTransform.position.x)
                    {
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                    else
                    {
                        transform.localScale = Vector3.one;
                    }
                }
            }
        }
    }
}