using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static bool isLifesteal = false;
    public static bool isMaximizer = false;

    public static PlayerController instance = null;

    public MiragePlayerController mpc = null;

    public FastDownEffect fastDownEffect = null;

    public TMP_Text hpText;
    public TMP_Text spText;

    [SerializeField] ContactFilter2D filter;
    [SerializeField] Image hpFillImage;
    [SerializeField] Image spFillImage;
    [SerializeField] Collider2D bottomCollider;
    [SerializeField] float[] moveSpeed;
    [SerializeField] float[] jumpSpeed;
    [SerializeField] float lastJumpSwing = -15f;
    [SerializeField] float rollRange = 5;
    [SerializeField] float rollSpeed = 2f;
    [SerializeField] float[] playerMaxHpBase;
    [SerializeField] float[] playerMaxSp;
    [SerializeField] float swingUseStamina = 15f;
    [SerializeField] float[] rollUseStamina;
    [SerializeField] float[] jumpUseStamina;
    [SerializeField] float[] staminaRegenSpeed;

    InputActionReference moveLeftInputAction = null;
    InputActionReference moveRightInputAction = null;
    InputActionReference moveDownInputAction = null;
    InputActionReference jumpInputAction = null;
    InputActionReference attackInputAction = null;
    InputActionReference rollInputAction = null;

    float playerMaxHp;

    [SerializeField] float fastDownAttackSpeed = -10f;

    Rigidbody2D rb;
    Animator ar;

    [HideInInspector] public Dir currentDir = Dir.Right;
    Dir beforeDir = Dir.Right;
    PlayerState ps = PlayerState.Idle;
    PlayerState pbs = PlayerState.Idle;
    Vector3 scale = Vector3.one;

    float playerHp = 10;
    float playerSp = 10;
    float beforeHpMaxRatio = 1f;

    int swingCombo = 0;
    int jumpSwingCombo = 0;

    bool invincibility = false; // true일 시 무적
    bool swingCheck = false;
    bool isStop = false;
    bool isAttacking = false;
    bool jumpUse = false;
    bool jumping = false;
    bool rolling = false;
    bool dieCheck = false;
    bool fastDownAttackCheck = false;
    bool fastDownAttacking = false;
    bool preventNextAction = false;
    [HideInInspector] public bool leaf = false; // 아이템으로 인해 스태미나 소모가 필요없을 때
    [HideInInspector] public bool shield = false; // 아이템으로 인해 공격을 막을 수 있을 때
    float PlayerSp {
        get { return playerSp; }
        set {
            playerSp = value;
            spFillImage.fillAmount =
                playerSp < 0 ?
                0 : playerSp / playerMaxSp[(int)Option.difficulty];
            spText.text = playerSp.ToString("F0") + " / " + playerMaxSp[(int)Option.difficulty].ToString();
        }
    }


    private float PlayerHp
    {
        get { return playerHp; }
        set
        {
            playerHp = Mathf.Clamp(value, 0, playerMaxHp);

            hpFillImage.fillAmount =
                playerHp < 0 ?
                0 : playerHp / playerMaxHp;
            hpText.text = playerHp.ToString("F0") + " / " + playerMaxHp.ToString();
        }
    }
    public bool IsAlive { get { return playerHp > 0; } }

    private void Awake()
    {
        instance = this;

        isLifesteal = false;
        isMaximizer = false;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ar = GetComponent<Animator>();

        playerMaxHp = playerMaxHpBase[(int)Option.difficulty];

        PlayerHp = playerMaxHp;
        PlayerSp = playerMaxSp[(int)Option.difficulty];
    }
    private void OnEnable()
    {
        moveLeftInputAction = InputManager.GetInputAction(InputType.LeftMove);
        moveRightInputAction = InputManager.GetInputAction(InputType.RightMove);
        moveDownInputAction = InputManager.GetInputAction(InputType.Down);
        jumpInputAction = InputManager.GetInputAction(InputType.Jump);
        attackInputAction = InputManager.GetInputAction(InputType.Attack);
        rollInputAction = InputManager.GetInputAction(InputType.Roll);
    }
    private void OnDisable()
    {
        InputManager.Release(InputType.LeftMove);
        InputManager.Release(InputType.RightMove);
        InputManager.Release(InputType.Down);
        InputManager.Release(InputType.Jump);
        InputManager.Release(InputType.Attack);
        InputManager.Release(InputType.Roll);
    }
    public void Update()
    {
        if (preventNextAction == true) return;

        if (jumpInputAction.action.WasPressedThisFrame())
        {
            if (preventNextAction) return;
            Jump();
        }
        else if (attackInputAction.action.WasPressedThisFrame())
        {
            if (preventNextAction) return;
            if (swingCheck == false) swingCheck = true;
        }
        else if (rollInputAction.action.WasPressedThisFrame())
        {
            if (preventNextAction) return;
            Roll();
        }
        else if (moveDownInputAction.action.WasPressedThisFrame())
        {
            FastDownAttack();
        }
    }
    void FastDownAttack()
    {
        if ((ps == PlayerState.Jump || ps == PlayerState.Fall) && !fastDownAttacking)
        {
            if (fastDownAttackCheck == false)
            {
                StartCoroutine(FastDownAttackDelayCoroutine());
            }
            else
            {
                StopCoroutine(FastDownAttackDelayCoroutine());

                fastDownAttacking = true;

                StartCoroutine(FastDownAttackCoroutine());
            }
        }
    }
    IEnumerator FastDownAttackCoroutine()
    {
        // 빠른 낙하 공격을 위한 기본 세팅

        rb.bodyType = RigidbodyType2D.Kinematic; // 중력 적용 X

        rb.linearVelocityY = fastDownAttackSpeed; // 속도 변화

        ps = PlayerState.Fall; // 떨어지는 상태로 전환

        while (transform.position.y > -3.2f) yield return null; // 땅에 근접할 때까지 반복

        SoundManager.Play("PlayerLandAttack", SoundType.Effect);

        fastDownEffect.Enable(transform.position);

        fastDownAttacking = false;

        ps = PlayerState.Idle; // 가만히 있는 상태로 전환

        rb.linearVelocityY = 0;

        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    IEnumerator FastDownAttackDelayCoroutine()
    {
        fastDownAttackCheck = true;

        yield return new WaitForSeconds(0.2f);

        fastDownAttackCheck = false;
    }
    public void StatusUpdate()
    {
        Debug.Log(beforeHpMaxRatio.ToString() + " / " + Inventory.CurrentData.playerHp);

        // 플레이어의 최대 HP를 늘렸을 때
        if (beforeHpMaxRatio != Inventory.CurrentData.playerHp)
        {
            beforeHpMaxRatio = Inventory.CurrentData.playerHp;

            float beforeHpMax = playerMaxHp;

            playerMaxHp = playerMaxHpBase[(int)Option.difficulty] * Inventory.CurrentData.playerHp;

            float distance = playerMaxHp - beforeHpMax;

            if (distance > 0) PlayerHp += distance;
        }
    }
    private void Jump()
    {
        // 현재 점프를 사용하지 않았다면
        if (jumpUse == false)
        {
            if (preventNextAction) return;
            if ((ps == PlayerState.Idle || ps == PlayerState.Run) && UseStamina(jumpUseStamina[(int)Option.difficulty]))
            {
                preventNextAction = true;

                ps = PlayerState.Jump;

                jumpUse = true;
            }
        }
    }
    private void Swing()
    {
        if (fastDownAttacking == true) return;

        if (preventNextAction) return;

        if (ps == PlayerState.Jump || ps == PlayerState.Fall || ps == PlayerState.JumpSwing1 || ps == PlayerState.JumpSwing2 || ps == PlayerState.JumpSwing3)
        {
            if (!rolling)
            {
                if (!isAttacking)
                {
                    if (UseStamina(swingUseStamina))
                    {
                        preventNextAction = true;

                        CancelInvoke();

                        isAttacking = true;

                        isStop = true;

                        if (jumpSwingCombo++ > 0)
                            if (jumpSwingCombo == 4)
                                jumpSwingCombo = 1;

                        ps = (PlayerState)(jumpSwingCombo + 12);

                        rb.linearVelocityY = 0;

                        rb.bodyType = RigidbodyType2D.Kinematic;
                    }
                }
            }
        }
        else if (!rolling)
        {
            if (!isAttacking && UseStamina(swingUseStamina))
            {
                preventNextAction = true;

                CancelInvoke();

                isAttacking = true;

                isStop = true;

                if (swingCombo++ > 0)
                    if (swingCombo == 4)
                        swingCombo = 1;

                ps = (PlayerState)(swingCombo + 9);
            }
        }

    }
    #region 소리 재생 함수
    void FootStapSoundPlay()
    {
        SoundManager.FootStapPlay();
    }
    void JumpSoundPlay()
    {
        SoundManager.Play("PlayerJump", SoundType.Effect);
    }
    void SwingSoundPlay_1()
    {
        SoundManager.Play("PlayerAttack1", SoundType.Effect);
    }
    void SwingSoundPlay_2()
    {
        SoundManager.Play("PlayerAttack2", SoundType.Effect);
    }
    void SwingSoundPlay_3()
    {
        SoundManager.Play("PlayerAttack3", SoundType.Effect);
    }
    #endregion
    private void FixedUpdate()
    {
        if (IsAlive == false)
        {
            if (dieCheck == false)
            {
                dieCheck = true;
                rb.linearVelocityY = 0;
                rb.linearVelocityX = 0;
            }
            return;
        }
        PlayerSp += Time.fixedDeltaTime * staminaRegenSpeed[(int)Option.difficulty] * Inventory.CurrentData.playerStaminaRegen;
        if (PlayerSp > playerMaxSp[(int)Option.difficulty]) PlayerSp = playerMaxSp[(int)Option.difficulty];


        if (ps == PlayerState.Fall || ps == PlayerState.Idle || ps == PlayerState.Run)
        {
            if (bottomCollider.IsTouching(filter))
            {
                if (ps == PlayerState.Fall)
                    SoundManager.Play("PlayerLand", SoundType.Effect);

                ps = PlayerState.Idle;
                jumping = false;
                jumpUse = false;
            }
        }


        if (jumpUse == true && jumping == false)
        {
            jumping = true;

            rb.linearVelocityY = 0;
            rb.AddForceY(jumpSpeed[(int)Option.difficulty], ForceMode2D.Impulse);
        }

        // 만약 현재 상태가 점프일 때,
        // 플레이어가 떨어지고 있으면
        // 현재 상태를 추락중으로 변경한다.
        if (ps == PlayerState.Jump)
            if (rb.linearVelocityY < 0)
                ps = PlayerState.Fall;

        float x = 0;
        if (moveLeftInputAction.action.IsPressed()) x -= 1;
        if (moveRightInputAction.action.IsPressed()) x += 1;

        if (!isStop)
        {
            if (x == 1) currentDir = Dir.Right;

            if (x == -1) currentDir = Dir.Left;

            if (currentDir != beforeDir)
            {
                beforeDir = currentDir;
                scale.x = (int)currentDir;
                transform.localScale = scale;
            }
            if (x != 0)
            {

                if (ps == PlayerState.Idle) ps = PlayerState.Run;

                rb.linearVelocityX = x * moveSpeed[(int)Option.difficulty] * Time.fixedDeltaTime * Inventory.CurrentData.playerMoveSpeed;
            }
            else if (ps == PlayerState.Run) ps = PlayerState.Idle;
        }
        else rb.linearVelocityX = 0;

        if (x == 0) rb.linearVelocityX = 0;

        // 공격 입력 부분
        if (swingCheck)
        {
            Swing();
            swingCheck = false;
        }

        // 현재 플레이어 상태가 갱신 되었을 때
        if (ps != pbs)
        {
            pbs = ps;

            preventNextAction = false;

            switch (ps)
            {
                case PlayerState.Idle: ar.SetTrigger("Idle"); break;
                case PlayerState.Run: ar.SetTrigger("Run"); break;
                case PlayerState.Roll: ar.SetTrigger("Roll"); break;
                case PlayerState.Fall: ar.SetTrigger("Fall"); break;
                case PlayerState.Jump: ar.SetTrigger("Jump"); break;
                case PlayerState.Swing1: ar.SetTrigger("Swing1"); if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
                case PlayerState.Swing2: ar.SetTrigger("Swing2"); if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
                case PlayerState.Swing3: ar.SetTrigger("Swing3"); if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
                case PlayerState.JumpSwing1: ar.SetTrigger("JumpSwing1"); if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
                case PlayerState.JumpSwing2: ar.SetTrigger("JumpSwing2");  if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
                case PlayerState.JumpSwing3: ar.SetTrigger("JumpSwing3");  if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
            }
        }
    }
    void RollEnd()
    {
        isStop = false;
        rolling = false;

        if (rb.linearVelocityY < 0) ps = PlayerState.Fall;

        else {
            ps = PlayerState.Idle;

            jumping = false;
            jumpUse = false;
        }
    }
    private void Roll()
    {
        if (fastDownAttacking == true) return;

        if (!rolling && (leaf || UseStamina(rollUseStamina[(int)Option.difficulty])))
        {

            SoundManager.Play("PlayerRoll", SoundType.Effect);

            leaf = false;
            isStop = true;
            rolling = true;
            isAttacking = false;

            ps = PlayerState.Roll;
            ar.SetTrigger("Roll");

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocityY = 0;

            StartCoroutine(RollCoroutine());
        }
    }
    IEnumerator RollCoroutine()
    {
        double t = 0;

        float startPos = transform.position.x;
        float endPos = transform.position.x + (rollRange * (int)currentDir * Inventory.CurrentData.rollRange);

        while (t < 1f)
        {
            t += Time.deltaTime * rollSpeed;
            transform.position = new Vector3(Mathf.Lerp(startPos, endPos, (float)t), transform.position.y, transform.position.z);
            yield return null;
        }
        transform.position = new Vector3(endPos, transform.position.y, transform.position.z);
    }
    void EnableCombo() // 콤보 공격 활성화 구간
    {
        isAttacking = false;

        Invoke("DisableCombo", .7f);
    }
    void EnableJumpCombo() // 콤보 공격 활성화 구간
    {
        isAttacking = false;

        Invoke("DisableJumpCombo", .7f);
    }
    void DisableCombo()
    {
        swingCombo = 0;
    }
    void DisableJumpCombo()
    {
        jumpSwingCombo = 0;
    }
    void EnableIdle() // 이동 및 회전 활성화 구간
    {
        ps = PlayerState.Idle;

        isStop = false;

        isAttacking = false;
    }
    void EnableJumpIdle() // 점프 공격 중에 이동 및 회전 활성화 구간
    {
        ps = PlayerState.Fall;

        isStop = false;

        isAttacking = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    void LastJumpSwing()
    {
        rb.linearVelocityY = lastJumpSwing;
    }
    public void TouchGround()
    {
        ps = PlayerState.Idle;

        isStop = false;

        isAttacking = false;

        rb.bodyType = RigidbodyType2D.Dynamic;

        rb.linearVelocityY = 0;

        jumpUse = false;

        jumping = false;
    }
    public void Hit(float damage)
    {
        if (IsAlive)
        {
            if (damage < 0) // 이것은 체력 회복
            {
                PlayerHp -= damage;

                DamageTextController.SetDamage(-damage, transform.position + (Vector3.up * 0.5f), 2);
            }
            else if (shield) // 방어 가능한 상태일 때
            {
                DamageTextController.Shield(transform.position + Vector3.up * 0.5f);
            }
            else if (!invincibility)
            {
                if (Random.Range(0, 101) < Inventory.CurrentData.playerAvoidChance)
                {
                    DamageTextController.Avoid(transform.position + Vector3.up * 0.5f);

                    ar.SetTrigger("Hit");

                    invincibility = true;
                }
                else if (!invincibility && !rolling)
                {
                    invincibility = true;

                    damage *= Inventory.CurrentData.monsterDamage;

                    if (Random.Range(0, 101) < Inventory.CurrentData.monsterCriticalChance)
                        damage *= 2f;

                    PlayerHp -= damage;

                    DamageTextController.SetDamage(damage, transform.position + (Vector3.up * 0.5f), 1);

                    if (PlayerHp <= 0) Die();
                    else ar.SetTrigger("Hit");

                    SoundManager.Play("PlayerHit", SoundType.Effect);
                }
            }
        }

    }
    void Die()
    {
        ar.SetBool("Die", true);
    }
    void OnDeadPanel()
    {
        GameManager.Instance.GameOver();
    }
    void NotInvincibility()
    {
        invincibility = false;
    }
    bool UseStamina(float cost)
    {
        if (PlayerSp > cost)
        {
            PlayerSp -= cost;

            return true;
        }
        return false;
    }
}
public enum Dir
{
    Left = -1,
    Right = 1
}
public enum PlayerState
{
    Idle,
    Run,
    Roll,
    Jump,
    Fall,
    Swing1 = 10,
    Swing2,
    Swing3,
    JumpSwing1,
    JumpSwing2,
    JumpSwing3
}