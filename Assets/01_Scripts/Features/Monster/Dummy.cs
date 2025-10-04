using UnityEngine;

public class Dummy : Monster
{
    public Transform playerTransform = null;
    public Animator animator;

    private void OnEnable()
    {
        Setting();
    }

    private void Start()
    {
        if (playerTransform == null) playerTransform = GameManager.Instance.player.transform;
    }

    private void FixedUpdate()
    {
        if (playerTransform.position.x > transform.position.x) sr.flipX = true;
        else sr.flipX = false;
    }

    public override void Hit(float damage, bool isMainAttack = true)
    {
        if (IsAlive)
        {
            damage *= Inventory.CurrentData.playerDamage;

            int damageType = 0;

            if (Random.Range(0, 101) <= Inventory.CurrentData.playerCriticalChance)
            {
                //여기부터 계속
                damage *= 2f;

                damageType = 1;
            }

            if (PlayerController.isMaximizer)
            {
                PlayerController.isMaximizer = false;

                MaximizerSupport.instance.OffMaximizer();

                damage *= 5f;

                damageType = 2;
            }

            if (Random.Range(0, 101) < Inventory.CurrentData.monsterAvoidChance)
            {
                // 몬스터 회피 시
                DamageTextController.Avoid(GetCenterPosition());
            }
            else
            {
                Hp -= damage;

                if (damageType == 0)
                {
                    DamageTextController.SetDamage(damage, GetCenterPosition());
                }
                else if (damageType == 1)
                {
                    DamageTextController.SetDamage(damage, GetCenterPosition(), 3);
                }
                else if (damageType == 2)
                {
                    DamageTextController.SetDamage(damage, GetCenterPosition(), 4);
                }

                GameManager.Instance.lastHitMonster = this;

                if (isMainAttack)
                    Inventory.AddStack();
            }
        }

        if (Hp == 0) Hp = maxHpBases[(int)Option.difficulty];

        animator.SetTrigger("Hit");
    }
}
