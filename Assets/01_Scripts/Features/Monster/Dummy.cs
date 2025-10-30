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

            DamageType damageType = DamageType.PlayerToMonsterNormal;

            if (Random.Range(0, 101) <= Inventory.CurrentData.playerCriticalChance)
            {
                //여기부터 계속
                damage *= 2f;

                damageType = DamageType.PlayerToMonsterCritical;
            }

            if (PlayerController.isMaximizer)
            {
                PlayerController.isMaximizer = false;

                MaximizerSupport.instance.OffMaximizer();

                damage *= 5f;

                damageType = DamageType.PlayerToMonsterMaxDamage;
            }

            if (Random.Range(0, 101) < Inventory.CurrentData.monsterAvoidChance)
            {
                // 몬스터 회피 시
                DamageTextController.Avoid(GetCenterPosition());
            }
            else
            {
                Hp -= damage;

                DamageTextController.SetDamage(damage, GetCenterPosition(), damageType);

                GameManager.Instance.lastHitMonster = this;

                if (isMainAttack) Inventory.AddStack();
            }
        }

        if (Hp == 0) Hp = maxHpBases[(int)Option.difficulty];

        animator.SetTrigger("Hit");
    }
}
