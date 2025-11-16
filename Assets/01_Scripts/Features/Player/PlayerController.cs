using System.Collections;
using TMPro;
using Unity.VisualScripting;
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

    InputActionReference moveLeftInputAction = null;
    InputActionReference moveRightInputAction = null;
    InputActionReference moveDownInputAction = null;
    InputActionReference jumpInputAction = null;
    InputActionReference attackInputAction = null;
    InputActionReference rollInputAction = null;

    float playerMaxHp;

    [SerializeField] float fastDownAttackSpeed = -15;

    Rigidbody2D rb;
    Animator ar;

    [HideInInspector] public Dir currentDir = Dir.Right;
    Dir beforeDir = Dir.Right;
    PlayerState ps = PlayerState.Idle;
    Vector3 scale = Vector3.one;

    float playerHp = 10;
    float playerSp = 10;
    float beforeHpMaxRatio = 1f;
    float PlayerSp {
        get { return playerSp; }
        set {
            playerSp = value;
            spFillImage.fillAmount =
                playerSp < 0 ?
                0 : playerSp / PlayerStat.Value.PlayerMaxSp;
            //spText.text = Mathf.FloorToInt(playerSp).ToString() + " / " + Mathf.FloorToInt(PlayerStat.Value.PlayerMaxSp).ToString();
            spText.SetText(PlayerPointFormat, Mathf.FloorToInt(playerSp), Mathf.FloorToInt(PlayerStat.Value.PlayerMaxSp));

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
            //hpText.text = playerHp.ToString("F0") + " / " + playerMaxHp.ToString();
            hpText.SetText(PlayerPointFormat, Mathf.FloorToInt(playerHp), Mathf.FloorToInt(playerMaxHp));
        }
    }
    public bool IsAlive { get { return playerHp > 0; } }

    private static readonly string PlayerPointFormat = "{0} / {1}";
    public PlayerState Ps
    {
        get => ps;

        set
        {
            if (ps == value) return;

            ps = value;

            switch (ps)
            {
                case PlayerState.Idle: ar.SetTrigger("Idle"); break;
                case PlayerState.Run: ar.SetTrigger("Run"); break;
                case PlayerState.Roll: ar.SetTrigger("Roll"); break;
                case PlayerState.Fall: ar.SetTrigger("Fall"); break;
                case PlayerState.Jump: ar.SetTrigger("Jump"); break;

                case PlayerState.Swing1:
                    ar.SetTrigger("Swing1");
                    if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); }
                    break;

                case PlayerState.Swing2:
                    ar.SetTrigger("Swing2");
                    if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); }
                    break;

                case PlayerState.Swing3:
                    ar.SetTrigger("Swing3");
                    if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); }
                    break;

                case PlayerState.JumpSwing1:
                    ar.SetTrigger("JumpSwing1");
                    if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); }
                    break;

                case PlayerState.JumpSwing2:
                    ar.SetTrigger("JumpSwing2");
                    if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); }
                    break;

                case PlayerState.JumpSwing3:
                    ar.SetTrigger("JumpSwing3");
                    if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); }
                    break;
            }
        }
    }
    private bool SetPlayerState(PlayerState ps)
    {
        if (Ps == ps || PlayerFlags.Value.TriggerLocked) return false;

        Ps = ps;

        PlayerFlags.Value.TriggerLocked = true;

        return true;
    }
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

        playerMaxHp = PlayerStat.Value.PlayerMaxHpBase;

        PlayerHp = playerMaxHp;
        PlayerSp = PlayerStat.Value.PlayerMaxSp;
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

        PlayerFlags.Value.ResetFlags();
    }
    public void Update()
    {
        PlayerFlags.Value.TriggerLocked = false;

        if (jumpInputAction.action.WasPressedThisFrame())
        {
            Jump();
        }
        else if (attackInputAction.action.WasPressedThisFrame())
        {
            if (PlayerFlags.Value.SwingCheck == false)
            {
                PlayerFlags.Value.SwingCheck = true;
            }
        }
        else if (rollInputAction.action.WasPressedThisFrame())
        {
            Roll();
        }
        else if (moveDownInputAction.action.WasPressedThisFrame())
        {
            FastDownAttack();
        }
    }
    Coroutine fastDownAttackCheckCoroutine = null;
    void FastDownAttack()
    {
        if ((Ps == PlayerState.Jump || Ps == PlayerState.Fall) && !PlayerFlags.Value.FastDownAttacking)
        {
            if (PlayerFlags.Value.FastDownAttackCheck == false)
            {
                fastDownAttackCheckCoroutine = StartCoroutine(FastDownAttackDelayCoroutine());
            }
            else
            {
                StopCoroutine(fastDownAttackCheckCoroutine);

                StartCoroutine(FastDownAttackCoroutine());
            }
        }
    }
    IEnumerator FastDownAttackCoroutine()
    {
        // 빠른 낙하 공격을 위한 기본 세팅
        if (Ps != PlayerState.Fall)
        {
            while (!SetPlayerState(PlayerState.Fall)) yield return null;
        }

        rb.bodyType = RigidbodyType2D.Kinematic; // 중력 적용 X

        rb.linearVelocityY = fastDownAttackSpeed; // 속도 변화

        PlayerFlags.Value.FastDownAttacking = true;

        while (transform.position.y > -3.2f) yield return null; // 땅에 근접할 때까지 반복

        SoundManager.Play("PlayerLandAttack", SoundType.Effect);

        fastDownEffect.Enable(transform.position);

        PlayerFlags.Value.FastDownAttackCheck = false;
        PlayerFlags.Value.FastDownAttacking = false;
        PlayerFlags.Value.JumpUse = false;
        PlayerFlags.Value.Jumping = false;

        rb.linearVelocityY = 0;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    IEnumerator FastDownAttackDelayCoroutine()
    {
        PlayerFlags.Value.FastDownAttackCheck = true;

        yield return new WaitForSeconds(0.2f);

        PlayerFlags.Value.FastDownAttackCheck = false;
    }
    public void StatusUpdate()
    {
        // 플레이어의 최대 HP를 늘렸을 때
        if (beforeHpMaxRatio != Inventory.CurrentData.playerHp)
        {
            beforeHpMaxRatio = Inventory.CurrentData.playerHp;

            float beforeHpMax = playerMaxHp;

            playerMaxHp = PlayerStat.Value.PlayerMaxHpBase * Inventory.CurrentData.playerHp;

            float distance = playerMaxHp - beforeHpMax;

            if (distance > 0) PlayerHp += distance;
        }
    }
    private void Jump()
    {
        // 현재 점프를 사용하지 않았다면
        if (PlayerFlags.Value.JumpUse == false)
        {
            if ((Ps == PlayerState.Idle || Ps == PlayerState.Run) && !PlayerFlags.Value.TriggerLocked && TryUseStamina(PlayerStat.Value.JumpUseStamina))
            {
                SetPlayerState(PlayerState.Jump);

                PlayerFlags.Value.JumpUse = true;
            }
        }
    }
    private void Swing()
    {
        if (PlayerFlags.Value.FastDownAttacking == true) return;

        if (Ps == PlayerState.Jump || Ps == PlayerState.Fall || Ps == PlayerState.JumpSwing1 || Ps == PlayerState.JumpSwing2 || Ps == PlayerState.JumpSwing3)
        {
            if (!PlayerFlags.Value.Rolling)
            {
                if (!PlayerFlags.Value.IsAttacking)
                {
                    if (!PlayerFlags.Value.TriggerLocked && TryUseStamina(PlayerStat.Value.SwingUseStamina))
                    {
                        CancelInvoke();

                        PlayerFlags.Value.IsAttacking = true;
                        PlayerFlags.Value.IsStop = true;

                        if (PlayerFlags.Value.JumpSwingCombo++ > 0)
                            if (PlayerFlags.Value.JumpSwingCombo == 4)
                                PlayerFlags.Value.JumpSwingCombo = 1;

                        SetPlayerState((PlayerState)(PlayerFlags.Value.JumpSwingCombo + 12));

                        rb.linearVelocityY = 0;

                        rb.bodyType = RigidbodyType2D.Kinematic;
                    }
                }
            }
        }
        else if (!PlayerFlags.Value.Rolling)
        {
            if (!PlayerFlags.Value.IsAttacking && !PlayerFlags.Value.TriggerLocked && TryUseStamina(PlayerStat.Value.SwingUseStamina))
            {
                CancelInvoke();

                PlayerFlags.Value.IsAttacking = true;
                PlayerFlags.Value.IsStop = true;

                if (PlayerFlags.Value.SwingCombo++ > 0)
                    if (PlayerFlags.Value.SwingCombo == 4)
                        PlayerFlags.Value.SwingCombo = 1;

                SetPlayerState((PlayerState)(PlayerFlags.Value.SwingCombo + 9));
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
    const string LandSoundName = "PlayerLand";
    private void FixedUpdate()
    {
        if (IsAlive == false)
        {
            if (PlayerFlags.Value.DieCheck == false)
            {
                PlayerFlags.Value.DieCheck = true;
                rb.linearVelocityY = 0;
                rb.linearVelocityX = 0;
            }
            return;
        }
        PlayerSp += Time.fixedDeltaTime * PlayerStat.Value.StaminaRegenSpeed * Inventory.CurrentData.playerStaminaRegen;
        if (PlayerSp > PlayerStat.Value.PlayerMaxSp) PlayerSp = PlayerStat.Value.PlayerMaxSp;

        if (Ps == PlayerState.Fall)
        {
            if (bottomCollider.IsTouching(filter))
            {
                SoundManager.Play(LandSoundName, SoundType.Effect);

                if (SetPlayerState(PlayerState.Idle))
                {
                    PlayerFlags.Value.Jumping = false;
                    PlayerFlags.Value.JumpUse = false;
                }
            }
        }


        if (PlayerFlags.Value.JumpUse == true && PlayerFlags.Value.Jumping == false)
        {
            PlayerFlags.Value.Jumping = true;

            rb.linearVelocityY = 0;
            rb.AddForceY(PlayerStat.Value.JumpSpeed, ForceMode2D.Impulse);
        }

        // 만약 현재 상태가 점프일 때,
        // 플레이어가 떨어지고 있으면
        // 현재 상태를 추락중으로 변경한다.
        if (Ps == PlayerState.Jump && rb.linearVelocityY < 0)
        {
            SetPlayerState(PlayerState.Fall);
        }

        float x = 0;

        if (moveLeftInputAction.action.IsPressed()) x -= 1;
        if (moveRightInputAction.action.IsPressed()) x += 1;

        if (!PlayerFlags.Value.IsStop)
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
                if (Ps == PlayerState.Idle)
                {
                    SetPlayerState(PlayerState.Run);
                }

                rb.linearVelocityX = x * PlayerStat.Value.MoveSpeed * Time.fixedDeltaTime * Inventory.CurrentData.playerMoveSpeed;
            }
            else if (Ps == PlayerState.Run)
            {
                SetPlayerState(PlayerState.Idle);
            }
        }
        else rb.linearVelocityX = 0;

        if (x == 0) rb.linearVelocityX = 0;

        // 공격 입력 부분
        if (PlayerFlags.Value.SwingCheck)
        {
            Swing();
            PlayerFlags.Value.SwingCheck = false;
        }
    }
    void RollEnd()
    {
        PlayerFlags.Value.IsStop = false;
        PlayerFlags.Value.Rolling = false;

        if (rb.linearVelocityY < 0)
        {
            SetPlayerState(PlayerState.Fall);
        }
        else
        {
            SetPlayerState(PlayerState.Idle);

            PlayerFlags.Value.Jumping = false;
            PlayerFlags.Value.JumpUse = false;
        }
    }
    private void Roll()
    {
        if (PlayerFlags.Value.FastDownAttacking == true) return;

        if (!PlayerFlags.Value.Rolling && !PlayerFlags.Value.TriggerLocked && (PlayerFlags.Value.Leaf || TryUseStamina(PlayerStat.Value.RollUseStamina)))
        {
            SetPlayerState(PlayerState.Roll);

            SoundManager.Play("PlayerRoll", SoundType.Effect);

            PlayerFlags.Value.Leaf = false;
            PlayerFlags.Value.IsStop = true;
            PlayerFlags.Value.Rolling = true;
            PlayerFlags.Value.IsAttacking = false;

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocityY = 0;

            StartCoroutine(RollCoroutine());
        }
    }
    IEnumerator RollCoroutine()
    {
        double t = 0;

        float startPos = transform.position.x;
        float endPos = transform.position.x + (PlayerStat.Value.RollRange * (int)currentDir * Inventory.CurrentData.rollRange);

        while (t < 1f)
        {
            t += Time.deltaTime * PlayerStat.Value.RollSpeed;
            transform.position = new Vector3(Mathf.Lerp(startPos, endPos, (float)t), transform.position.y, transform.position.z);
            yield return null;
        }
        transform.position = new Vector3(endPos, transform.position.y, transform.position.z);
    }
    void EnableCombo() // 콤보 공격 활성화 구간
    {
        PlayerFlags.Value.IsAttacking = false;

        Invoke("DisableCombo", .7f);
    }
    void EnableJumpCombo() // 콤보 공격 활성화 구간
    {
        PlayerFlags.Value.IsAttacking = false;

        Invoke("DisableJumpCombo", .7f);
    }
    void DisableCombo()
    {
        PlayerFlags.Value.SwingCombo = 0;
    }
    void DisableJumpCombo()
    {
        PlayerFlags.Value.JumpSwingCombo = 0;
    }
    void EnableIdle() // 이동 및 회전 활성화 구간
    {
        SetPlayerState(PlayerState.Idle);

        PlayerFlags.Value.IsStop = false;
        PlayerFlags.Value.IsAttacking = false;
    }
    void EnableJumpIdle() // 점프 공격 중에 이동 및 회전 활성화 구간
    {
        SetPlayerState(PlayerState.Fall);

        rb.bodyType = RigidbodyType2D.Dynamic;

        PlayerFlags.Value.IsStop = false;
        PlayerFlags.Value.IsAttacking = false;
    }
    void LastJumpSwing()
    {
        rb.linearVelocityY = PlayerStat.Value.LastJumpSwing;
    }
    public void TouchGround()
    {
        SetPlayerState(PlayerState.Idle);

        rb.bodyType = RigidbodyType2D.Dynamic;

        rb.linearVelocityY = 0;

        PlayerFlags.Value.IsStop = false;
        PlayerFlags.Value.IsAttacking = false;
        PlayerFlags.Value.JumpUse = false;
        PlayerFlags.Value.Jumping = false;
    }
    public void Hit(float damage)
    {
        if (IsAlive)
        {
            if (damage < 0) // 이것은 체력 회복
            {
                PlayerHp -= damage;

                DamageTextController.SetDamage(-damage, transform.position + (Vector3.up * 0.5f), DamageType.PlayerHealing);
            }
            else if (PlayerFlags.Value.Shield) // 방어 가능한 상태일 때
            {
                DamageTextController.Shield(transform.position + Vector3.up * 0.5f);
            }
            else if (!PlayerFlags.Value.Invincibility)
            {
                if (Random.Range(0, 101) < Inventory.CurrentData.playerAvoidChance)
                {
                    DamageTextController.Avoid(transform.position + Vector3.up * 0.5f);

                    ar.SetTrigger("Hit");

                    PlayerFlags.Value.Invincibility = true;
                }
                else if (!PlayerFlags.Value.Invincibility && !PlayerFlags.Value.Rolling)
                {
                    PlayerFlags.Value.Invincibility = true;

                    damage *= Inventory.CurrentData.monsterDamage;

                    if (Random.Range(0, 101) < Inventory.CurrentData.monsterCriticalChance) damage *= 2f;

                    PlayerHp -= damage;

                    DamageTextController.SetDamage(damage, transform.position + (Vector3.up * 0.5f), DamageType.MonsterToPlayerNormal);

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
        PlayerFlags.Value.Invincibility = false;
    }
    bool TryUseStamina(float cost)
    {
        if (PlayerSp > cost)
        {
            PlayerSp -= cost;

            return true;
        }
        return false;
    }
    //bool HasEnoughStamina(float cost)
    //{
    //    return PlayerSp >= cost;
    //}
}
public enum Dir
{
    Left = -1,
    Right = 1
}