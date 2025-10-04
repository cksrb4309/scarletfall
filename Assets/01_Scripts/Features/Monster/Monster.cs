using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Monster : MonoBehaviour
{
    public int id;
    public string mobName;
    public bool isGroundMob = false;

    [SerializeField] protected Portal portal;
    [SerializeField] protected SpriteRenderer sr;

    [SerializeField] protected Collider2D cd;
    [SerializeField] protected GameObject hpBarParent;
    [SerializeField] protected Image hpBar;

    [SerializeField] protected float[] maxHpBases;

    [SerializeField] protected float heightOffset = 0f;
    
    protected float maxHp;
    float hp;

    [HideInInspector] public bool isBattle = false;
    protected float Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            
            if (hp < 0) hp = 0;

            if (hp != 0) hpBar.fillAmount = hp / maxHp;

            else hpBar.fillAmount = 0;
        }
    }

    private float y = -1f;

    protected float Height
    {
        get
        {
            if (y < -0.5f) y = sr.bounds.size.y / 2f;

            return y;
        }
    }

    protected bool IsAlive
    {
        get
        {
            return hp > 0;
        }
    }

    public virtual void Hit(float damage, bool isMainAttack = true)
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

                SoundManager.Play("Maximizer", SoundType.Effect);

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

                if (PlayerController.isLifesteal)
                {
                    float value = damage * 0.03f;

                    HealObj healObj = PoolingManager.Instance.GetObject<HealObj>("HealObj");

                    healObj.transform.position = GetCenterPosition();

                    healObj.Setting(value);
                }
            }
        }
    }
    public virtual void OnDisable()
    {
        EndMonster();
    }
    public void Setting()
    {
        if (Inventory.CurrentData != null)
            maxHp = maxHpBases[(int)Option.difficulty] * Inventory.CurrentData.monsterHp;
        else
            maxHp = maxHpBases[(int)Option.difficulty];

        Hp = maxHp;

        cd.enabled = false;

        sr.enabled = false;

        hpBarParent.SetActive(false);

        portal.gameObject.SetActive(true);

        portal.transform.position = GetCenterPosition();
    }
    public virtual void StartMonster()
    {
        cd.enabled = true;

        sr.enabled = true;

        hpBarParent.SetActive(true);
    }
    public virtual void EndMonster()
    {
        if (isBattle)
        {
            Battle.instance.KillMonster();

            Battle.instance.RemoveMonsterTransform(id);

            isBattle = false;
        }
    }
    public Vector3 GetCenterPosition()
    {
        return transform.position + (Vector3.up * (Height + heightOffset));
    }
}