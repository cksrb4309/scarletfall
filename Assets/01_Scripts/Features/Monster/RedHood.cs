using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class RedHood : Monster
{
    public float moveTowardPlayerRatio = 1f;

    public float nextActionDelay = 1f;

    public Animator ar;

    public Transform throwKnifeFirePos;

    public Transform[] trackingArrowFirePos;

    public ParticleSystem[] particles;
    Transform Player
    {
        get { return PlayerController.instance.transform; }
    }

    Dir dir // 플레이어의 위치보다 오른편에 있어서 왼쪽을 바라볼 때는 Left, 오른쪽을 쳐다보면 Right
    {
        get
        {
            return transform.position.x > Player.position.x ?
                Dir.Left : Dir.Right;
        }
    }

    int phase = 0; // 현재 보스몹 단계 Phase
    int pattern = 0; // 랜덤으로 선택한 패턴
    int current = -1; // 현재 패턴 액션 Index

    Vector3 beforePos = new Vector3(0, 3.255901f, 0); // 이전 위치 저장
    Vector3 currentPosition = Vector3.zero; // 현재 위치

    Action[][][] patternActions; // 패턴별 행동을 순차적으로 담은 Action 가변 배열

    private void Awake()
    {
        #region patternActions 페이즈, 패턴 크기에 맞게 초기화

        patternActions = new Action[3][][];

        patternActions[0] = new Action[3][];
        patternActions[1] = new Action[3][];
        patternActions[2] = new Action[3][];

        patternActions[0][0] = new Action[5];
        patternActions[0][1] = new Action[5];
        patternActions[0][2] = new Action[5];

        patternActions[1][0] = new Action[4];
        patternActions[1][1] = new Action[4];
        patternActions[1][2] = new Action[4];

        patternActions[2][0] = new Action[5];
        patternActions[2][1] = new Action[5];
        patternActions[2][2] = new Action[5];

        #endregion

        #region 보스몹의 패턴 설정

        #region 1 Phase Pattern Set
        patternActions[0][0][0] = FastKnifeAttack;
        patternActions[0][0][1] = ThrowPoisonApple;
        patternActions[0][0][2] = KnifeAttack;
        patternActions[0][0][3] = ThrowPoisonApple;
        patternActions[0][0][4] = StealthKnifeAttack;

        patternActions[0][1][0] = FastKnifeAttack;
        patternActions[0][1][1] = ThrowPoisonApple;
        patternActions[0][1][2] = KnifeAttack;
        patternActions[0][1][3] = ThrowPoisonApple;
        patternActions[0][1][4] = StealthKnifeAttack;

        patternActions[0][2][0] = FastKnifeAttack;
        patternActions[0][2][1] = ThrowPoisonApple;
        patternActions[0][2][2] = KnifeAttack;
        patternActions[0][2][3] = ThrowPoisonApple;
        patternActions[0][2][4] = StealthKnifeAttack;
        #endregion

        #region 2 Phase Pattern Set
        patternActions[1][0][0] = AxeAttack;
        patternActions[1][0][1] = StealthKnifeAttack;
        patternActions[1][0][2] = FastKnifeAttack;
        patternActions[1][0][3] = StealthBackMoveThrowKnife;

        patternActions[1][1][0] = FastKnifeAttack;
        patternActions[1][1][1] = AxeAttack;
        patternActions[1][1][2] = StealthKnifeAttack;
        patternActions[1][1][3] = StealthBackMoveThrowKnife;

        patternActions[1][2][0] = StealthKnifeAttack;
        patternActions[1][2][1] = AxeAttack;
        patternActions[1][2][2] = FastKnifeAttack;
        patternActions[1][2][3] = StealthBackMoveThrowKnife;
        #endregion

        #region 3 Phase Pattern Set
        patternActions[2][0][0] = AxeAttack;
        patternActions[2][0][1] = ChargeAxeAttack;
        patternActions[2][0][2] = ChargeArrow;
        patternActions[2][0][3] = ChargeAxeAttack;
        patternActions[2][0][4] = ChargeArrow;

        patternActions[2][1][0] = TripleTrackingArrow;
        patternActions[2][1][1] = ChargeAxeAttack;
        patternActions[2][1][2] = ChargeAxeAttack;
        patternActions[2][1][3] = ChargeArrow;
        patternActions[2][1][4] = ChargeAxeAttack;

        patternActions[2][2][0] = AxeAttack;
        patternActions[2][2][1] = ChargeAxeAttack;
        patternActions[2][2][2] = ChargeAxeAttack;
        patternActions[2][2][3] = ChargeArrow;
        patternActions[2][2][4] = TripleTrackingArrow;
        #endregion

        #endregion

        currentPosition = transform.position;
    }
    public override void StartMonster()
    {
        base.StartMonster();

        currentPosition = transform.position;

        // patternActions[phase][pattern][current].Invoke();

        ar.SetTrigger("Start");

        particles[10].Play();

        cd.enabled = false;

        SoundManager.Play("RedHoodLaugh", SoundType.Effect);
    }
    #region 삼중 추적 화살                                [ TripleTrackingArrow ]

    int trackingArrowFirePosIndex = 0; // trackingArrowFirePos[]의 주소로 사용하는 Index 값
    void TripleTrackingArrow()
    {
        Debug.Log("삼중 추적 화살 시작");

        ar.SetTrigger("TripleArrowShoot");

        trackingArrowFirePosIndex = 0;
    }
    void TrackingArrowShoot()
    {
        Debug.Log("삼중 추적 화살 발사");

        TrackingArrow arrow = PoolingManager.Instance.GetObject<TrackingArrow>("TrackingArrow");

        arrow.StartMove(trackingArrowFirePos[trackingArrowFirePosIndex++].position);

        SoundManager.Play("RedHoodTrackingCharge", SoundType.Effect);
    }
    #endregion
    #region 활을 시위에 겨누고 이동 후 정면 방향에 쏜다   [ ChargeArrow ]
    void ChargeArrow()
    {
        Debug.Log("충전 시작");
        ar.SetTrigger("ChargeArrow");
    }
    void ChargeArrowEffect()
    {
        Debug.Log("충전 이펙트");
        particles[3].Play(); // 활로 충전하는 이펙트 줌

        SoundManager.Play("ChargeArrow", SoundType.Effect);
    }
    void CheckDash() // 이동해야할 필요가 있을 때 이동한 후 화살을 쏜다
    {
        Debug.Log("이동확인");
        if (Mathf.Abs(Player.position.x - currentPosition.x) < 3)
        {
            particles[0].Play(); // 연막 재생

            while (Mathf.Abs(Player.position.x - currentPosition.x) < 5f || currentPosition.x > 10f || currentPosition.x < -11f)
            {
                currentPosition.x = UnityEngine.Random.Range(-11f, 10f);
            }
            transform.position = currentPosition;

            particles[0].Play(); // 연막 재생
        }
        LookAt(); // 플레이어 쳐다보기

        ChargeArrowShoot(); // 화살 발사
    }
    void ChargeArrowShoot()
    {
        Debug.Log("화살 발사");

        SoundManager.Play("ShootArrow", SoundType.Effect);

        Vector3 front = (dir == Dir.Left ? Vector3.left : Vector3.right);
        Vector3 top = Vector3.up;

        for (int t = 0; t < 6; t++)
        {
            RedHoodArrow arrow = PoolingManager.Instance.GetObject<RedHoodArrow>("RedHoodArrow");

            arrow.transform.position = throwKnifeFirePos.position;

            arrow.StartMove(12f, Vector3.Lerp(front, top, t != 0 ? t / 5f : 0));
        }
        ar.SetTrigger("ShootArrow");
    }
    #endregion
    #region 집중한 이후에 정면방향으로 도끼를 크게 휘두름 [ ChargeAxeAttack ]
    void ChargeAxeAttack()
    {
        Debug.Log("도끼 충전 공격");
        ar.SetTrigger("AxeGreatAttackWaiting"); // 준비모션

        particles[2].Play(); // 충천 이펙트? 표시

        LookAt();
    }
    void ChargeAxe()
    {
        LookAt();

        Debug.Log("휘두르기 전 X:" + currentPosition.x.ToString());

        currentPosition.x += (dir == Dir.Left ? -8.74f : 8.74f);

        Debug.Log("휘두르기 후 X:" + currentPosition.x.ToString());

        transform.position = currentPosition;

        Debug.Log("위치 확인 X:" + transform.position.x.ToString());

        SoundManager.Play("RedHoodGreatAxeSwing", SoundType.Effect);

        ar.SetTrigger("AxeGreatAttack");
    }
    void DelayNextActionSetting(float delay)
    {
        StartCoroutine(DelayNextActionSettingCoroutine(delay));
    }
    IEnumerator DelayNextActionSettingCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        NextActionSetting(-1);
    }
    #endregion
    #region 은신 후 이동 후 플레이어에게 단검을 던짐      [ StealthBackMoveThrowKnife ]
    void StealthBackMoveThrowKnife()
    {
        StartCoroutine(StealthBackMoveThrowKnifeCoroutine());
    }
    IEnumerator StealthBackMoveThrowKnifeCoroutine()
    {
        particles[0].Play(); // 연막 이펙트 재생

        sr.enabled = false; // 빨간 망토 숨기기

        cd.enabled = false; // 충돌 비활성화

        hpBarParent.SetActive(false); // hp바 숨기기

        currentPosition = currentPosition + (Vector3.right * (currentPosition.x > 0 ? -10 : 10));

        while (Mathf.Abs(currentPosition.x - Player.position.x) < 8f)
        {
            currentPosition.x = UnityEngine.Random.Range(-10f, 10f);
        }

        float t = 0;

        Vector3 start = transform.position;
        Vector3 move = start;

        while (t < 1f)
        {
            t += Time.deltaTime;

            move.x = Mathf.Lerp(start.x, currentPosition.x, t);

            transform.position = move;

            yield return null;
        }
        transform.position = currentPosition;

        sr.enabled = true; // 빨간 망토 표시

        particles[0].Play(); // 연막 이펙트 재생

        cd.enabled = true; // 충돌 활성화

        hpBarParent.SetActive(true); // hp바 표시

        LookAt(); // 플레이어 쳐다보게 하기

        ar.SetTrigger("ThrowKnife"); // 공격 모션
    }
    void ThrowKnife()
    {
        // 단검 생성
        RedHoodKnife knife = PoolingManager.Instance.GetObject<RedHoodKnife>("RedHoodKnife");

        // 단검 위치 조정
        knife.transform.position = throwKnifeFirePos.position;

        // 단검 속도, 방향 조정
        knife.StartMove(12f, ((Player.position + Vector3.up * 0.5f) - knife.transform.position).normalized);

        SoundManager.Play("RedHoodThrowKnife", SoundType.Effect);
    }
    #endregion
    #region 플레이어 쳐다본 후 해당 방향으로 이동         [ MoveTowardPlayer ]
    void MoveTowardPlayer()
    {
        LookAt(); // 플레이어 쳐다보기

        // 차이가 좀 벌어져있을 때
        if (Mathf.Abs(currentPosition.x - Player.position.x) > 2f)
            particles[1].Play(); // 대쉬 이펙트 재생

        ForwardMove(Mathf.Abs(transform.position.x - Player.position.x) + (dir == Dir.Left ? -1f : 1f)); // 플레이어와의 거리 차를 가져와서 이동시킴
    }
    #endregion
    #region 이동 후 도끼 공격                             [ AxeAttack ]
    void AxeAttack()
    {
        Debug.Log("도끼 공격");

        StartCoroutine(AxeAttackCoroutine());
    }
    IEnumerator AxeAttackCoroutine()
    {
        yield return null;

        ar.SetTrigger("AxeAttack");
    }
    #endregion
    #region 이동 후 공격                                  [ KnifeAttack ]
    void KnifeAttack()
    {
        Debug.Log("평범 이동 공격");
        // 플레이어에게 평범히 이동하여 단검 3타 공격
        StartCoroutine(KnifeAttackCoroutine());
    }
    IEnumerator KnifeAttackCoroutine()
    {
        Debug.Log("KnifeAttackCoroutine");

        // 플레이어와의 가로 거리 확인할 float
        float distance = Mathf.Abs(transform.position.x - Player.position.x);

        LookAt(); // 플레이어 쳐다보기

        // 뛰는 애니메이션으로 전환
        if (distance > 1.2f) ar.SetTrigger("Run");

        while (distance > 1.2f) // 가까워 질때까지 실행
        {
            // 플레이어의 위치에 따라 x축 이동을 한다
            currentPosition.x += (dir == Dir.Right ? 1 : -1) * 5f * Time.deltaTime;

            // 위치를 적용한다
            transform.position = currentPosition;

            // 차이 값을 구한다
            distance = Mathf.Abs(transform.position.x - Player.position.x);

            yield return null;
        }

        // 공격 애니메이션 실행
        ar.SetTrigger("KnifeAttack");
    }
    #endregion
    #region 빠른 이동 후 공격                             [ FastKnifeAttack ]
    void FastKnifeAttack()
    {
        Debug.Log("빠른 이동 공격");
        // 플레이어에게 빠른 속도로 이동하여 단검 3타 공격
        StartCoroutine(FastKnifeAttackCoroutine());
    }
    IEnumerator FastKnifeAttackCoroutine()
    {
        Debug.Log("FastKnifeAttackCoroutine");

        LookAt(); // 플레이어 쳐다보기

        // 공격 준비 자세 취하기
        ar.SetTrigger("FastKnifeAttackWaiting");

        float t = 1;
        Color color = Color.white;
        while (t > 0.2f)
        {
            yield return null;

            t -= Time.deltaTime * 2f;

            color.a = t;

            sr.color = color;
        }

        beforePos = currentPosition;

        bool isLeft = UnityEngine.Random.value > 0.5f;

        currentPosition.x = Player.position.x + (isLeft ? 1.2f : -1.2f);
        currentPosition.y = Player.position.y;

        transform.position = currentPosition;

        LookAt();

        t = 0.5f;

        color.a = 0.5f;

        sr.color = color;

        while (t < 1f)
        {
            yield return null;

            t += Time.deltaTime * 2f;

            color.a = t;

            sr.color = color;
        }

        // 공격 애니메이션 실행
        ar.SetTrigger("FastKnifeAttack");
    }
    void LastFastKnifeAttack()
    {
        StartCoroutine(LastFastKnifeAttackCoroutine());
    }
    IEnumerator LastFastKnifeAttackCoroutine()
    {
        Color color = Color.white;

        float t = 1f;

        while (t > 0.2f)
        {
            yield return null;

            t -= Time.deltaTime * 4f;

            color.a = t;

            sr.color = color;
        }

        currentPosition = beforePos;

        transform.position = currentPosition;

        t = 0.2f;

        LookAt();

        while (t < 1f)
        {
            yield return null;

            t += Time.deltaTime * 4f;

            color.a = t;

            sr.color = color;
        }

        NextActionSetting();
    }
    #endregion
    #region 은신 후 치명타 가하기                         [ StealthKnifeAttack ]
    void StealthKnifeAttack()
    {
        Debug.Log("은신 치명타");
        StartCoroutine(StealthKnifeAttackCoroutine());
    }
    IEnumerator StealthKnifeAttackCoroutine()
    {
        // 공격 준비 자세 취하기
        ar.SetTrigger("StealthAttackWaiting");

        yield return new WaitForSeconds(0.5f); // 자세 취한 후 0.5초 딜레이

        particles[0].Play(); // 연막 이펙트 재생

        sr.enabled = false; // 빨간 망토 숨기기

        cd.enabled = false; // 충돌 비활성화

        hpBarParent.SetActive(false); // hp바 숨기기

        yield return new WaitForSeconds(1f); // 연막 재생, 표시 가리고 1초 후

        float t = 0;

        bool isLeft = UnityEngine.Random.value > 0.5f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(Player.position.x + (isLeft ? -1.2f : 1.2f), Player.position.y, 1);
        Vector3 middlePos = new Vector3(startPos.x + targetPos.x / 2f, isLeft ? 3f : -3f, 1);

        while (t < 1f)
        {
            yield return null;
            t += Time.deltaTime;

            middlePos.Set(startPos.x + targetPos.x / 2f, isLeft ? 3f : -3f, 1);
            targetPos.Set(Player.position.x + (isLeft ? -1.2f : 1.2f), Player.position.y, 1);
            currentPosition = Vector3.Lerp(Vector3.Lerp(startPos, middlePos, t), Vector3.Lerp(middlePos, targetPos, t), t);
            transform.position = currentPosition;
        }

        sr.enabled = true; // 빨간 망토 표시

        particles[0].Play(); // 연막 이펙트 재생

        cd.enabled = true; // 충돌 활성화

        hpBarParent.SetActive(true); // hp바 표시

        LookAt(); // 플레이어 쳐다보게 하기

        // 공격 애니메이션 실행
        ar.SetTrigger("StealthAttack");
    }
    #endregion
    #region 사과를 던지면서 이동                          [ ThrowPoisonApple ]
    void ThrowPoisonApple()
    {
        Debug.Log("사과 시작");
        StartCoroutine(ThrowPoisonAppleCoroutine());
    }
    IEnumerator ThrowPoisonAppleCoroutine()
    {
        Debug.Log("ThrowPoisonAppleCoroutine");

        yield return null;

        ar.SetTrigger("ThrowAppleRun"); // 달리기 애니메이션

        float goal = (10f * (transform.position.x <= 0 ? 1 : -1));

        float baseDelay = 0.3f;
        float delay = 0.5f;

        float current = 0;

        bool isLeft = goal < 0;
        bool dirCheck = !isLeft;

        LookAt(0);

        while (true)
        {
            currentPosition.x += Time.deltaTime * 5f * (isLeft ? -1 : 1);

            transform.position = currentPosition;

            goal += (isLeft ? 1 : -1) * Time.deltaTime * 5f;

            isLeft = goal < 0;

            current += Time.deltaTime;

            if (current > delay)
            {
                delay += baseDelay;

                GameObject poisonApple = PoolingManager.Instance.GetObject("PoisonApple");

                poisonApple.transform.position = transform.position + Vector3.up * 0.5f;

                SoundManager.Play("RedHoodThrowApple", SoundType.Effect);
            }

            if (dirCheck == isLeft) break;

            yield return null;
        }

        NextActionSetting();
    }
    #endregion
    #region 정면으로 일정량만큼 이동                      [ ForwardMove ]
    void ForwardMove(float moveValue)
    {
        Debug.Log("이동량 발생");
        StartCoroutine(ForwardMoveCoroutine(moveValue));
    }
    IEnumerator ForwardMoveCoroutine(float moveValue)
    {
        float st = currentPosition.x;

        float ed = currentPosition.x + moveValue * (transform.localScale.x > 0 ? -1 : 1);

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 5f;

            currentPosition.x = Mathf.Lerp(st, ed, t);

            transform.position = currentPosition;

            yield return null;
        }
    }
    #endregion
    #region 플레이어 혹은 무언가를 바라보기               [ LookAt ]
    void LookAt(float pos = float.MaxValue)
    {
        if (pos != float.MaxValue)
            transform.localScale = new Vector3(transform.position.x > pos ? 1 : -1, 1, 1);
        else
            transform.localScale = new Vector3(dir == Dir.Left ? 1 : -1, 1, 1);
    }
    #endregion
    #region 다음 행동 결정                                [ NextActionSetting ]
    void NextActionSetting(float delay = 0)
    {
        if (cd.enabled == false)
        {
            cd.enabled = true;
        }
        current++; // 현재 패턴에서 다음 액션으로 넘긴다

        // 만약 현재 패턴이 끝났을 경우
        if (current >= patternActions[phase][pattern].Length)
        {
            // 랜덤으로 패턴 하나를 선택 한다
            pattern = UnityEngine.Random.Range(0, patternActions[phase].Length);

            // 첫 부분 액션으로 초기화 한다
            current = 0;
        }

        if (currentPosition.y > -3.255901f)
        {
            StartCoroutine(FallCoroutine());
        }
        else
        {
            StartCoroutine(NextActionSettingCoroutine(delay));
        }
    }
    IEnumerator FallCoroutine()
    {
        ar.SetTrigger("Fall");

        while (currentPosition.y > -3.255901f)
        {
            currentPosition.y -= Time.deltaTime * 4f;

            transform.position = currentPosition;

            yield return null;
        }
        currentPosition.y = -3.255901f;

        transform.position = currentPosition;

        ar.SetTrigger("Land");

        DelayNextActionSetting(1f);
    }
    IEnumerator NextActionSettingCoroutine(float delay)
    {
        if (!IsAlive) // 체력이 0 이하가 됐을 경우
        {
            // Phase가 2일 경우에는 마지막 페이즈이므로 Die 
            if (phase == 2) DieSetting();

            // Phase가 0,1일 경우에는 남은 페이즈가 있으므로 NextPhase
            else NextPhaseSetting();
        }
        else
        {
            // delay가 0보다 크다면 기본 딜레이를 무시하고 delay 동안 기다린다
            if (delay > 0) yield return new WaitForSeconds(delay);

            // 기본값인 0이라면 기본 ActionDelay를 적용한다
            else if (delay == 0) yield return new WaitForSeconds(nextActionDelay);

            yield return null;

            patternActions[phase][pattern][current].Invoke();
        }
    }
    #endregion
    public override void Hit(float damage, bool isMainAttack = true)
    {
        base.Hit(damage, isMainAttack);
    }
    void NextPhaseSetting()
    {
        phase++; // 페이즈를 한단계 높인다

        cd.enabled = false; // 충돌 비활성화

        Debug.Log("현재 Phase : " + phase.ToString());

        // 랜덤으로 패턴 하나를 선택 한다
        pattern = UnityEngine.Random.Range(0, patternActions[phase].Length);

        current = 0; // 패턴의 순서를 0으로 초기화

        if (phase == 1) // 중간 페이즈일 경우
        {
            ar.SetTrigger("Phase_1");

            particles[4].Play(); // Phase_1 Effect 재생
        }
        else // 마지막 페이즈일 경우
        {
            ar.SetTrigger("Phase_2");

            particles[5].Play(); // Phase_2 Effect 재생
            particles[6].Play(); 
            particles[7].Play(); 
        }

        StartCoroutine(FillHpCoroutine());
    }
    IEnumerator FillHpCoroutine()
    {
        maxHp = phase == 1 ? maxHpBases[(int)Option.difficulty] * 1.2f : maxHpBases[(int)Option.difficulty] * 1.5f;
        maxHp *= Inventory.CurrentData.monsterHp;

        float t = 0;

        while(t < 1f)
        {
            t += Time.deltaTime * 0.5f;

            Hp = Mathf.Lerp(0, maxHp, t);

            yield return null;
        }

        Hp = maxHp;

        cd.enabled = true; // 충돌 활성화
    }
    void NextPhaseActionSelect()
    {
        // Phase 1일 때는 환영 인사로 도끼 공격을 시전한다
        if (phase == 1) AxeAttack();

        // Phase 2일 때는 환영 인사로 도끼 강공격을 시전한다
        else ChargeAxeAttack();
    }
    void DieSetting()
    {
        ar.SetTrigger("Die");

        particles[8].Play();
        particles[9].Play();

        SoundManager.Play("RedHoodScream", SoundType.Effect);
    }
    void ClearGame()
    {
        GameManager.Instance.GameClear();
    }
    void KnifeSwingSoundPlay1()
    {
        SoundManager.Play("RedHoodKnifeSwing1", SoundType.Effect);
    }
    void KnifeSwingSoundPlay2()
    {
        SoundManager.Play("RedHoodKnifeSwing2", SoundType.Effect);
    }
    void KnifeSwingSoundPlay3()
    {
        SoundManager.Play("RedHoodKnifeSwing3", SoundType.Effect);
    }
    void CriticalKnifeSwingSoundPlay()
    {
        SoundManager.Play("RedHoodCriticalKnife", SoundType.Effect);
    }
}