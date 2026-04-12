# Scarletfall

`빨간 망토의 비극`은 10개 스테이지와 최종 보스전으로 구성된 2D 액션 로그라이크 프로젝트입니다.  
이 문서는 게임 소개보다, 제가 실제로 구현한 기능과 그 기능을 어떤 코드 구조로 만들었는지 보여주는 개발자 포트폴리오 중심으로 정리했습니다.

## 기술 스택

`Unity` `C#` `ScriptableObject` `Input System` `URP`

## 구현한 핵심 기능

- 플레이어 콤보 공격, 공중 공격, 구르기, 급강하 공격 구현
- 스테이지 클리어 후 3선택 보상과 스탯 반영 구조 구현
- 잔상 공격, 투사체, 추적 투사체, 낙뢰, 연쇄 번개 등 능력 효과 구현
- 3페이즈 보스 패턴과 엔딩 UI까지 연결되는 최종전 구현

## 1. 플레이어 전투 기능

- 입력에 따라 점프, 기본 공격, 구르기, 급강하 공격이 동작하도록 구현했습니다.
- 지상 콤보와 공중 공격을 별도 상태로 관리해 액션 흐름이 자연스럽게 이어지도록 구성했습니다.
- 스태미나를 전투 행동과 연결해 공격과 회피의 선택이 플레이 감각에 반영되도록 만들었습니다.

파일:
- `Assets/01_Scripts/Features/Player/PlayerController.cs`
- `Assets/01_Scripts/Features/Player/PlayerFlags.cs`

<details>
  <summary><strong>코드 보기</strong></summary>

```csharp
public void Update()
{
    // 매 프레임 입력을 읽기 전에 상태 잠금을 초기화한다.
    PlayerFlags.Value.TriggerLocked = false;

    // 입력 우선순서에 따라 점프, 공격, 구르기, 급강하 공격을 분기한다.
    if (jumpInputAction.action.WasPressedThisFrame())
    {
        Jump();
    }
    else if (attackInputAction.action.WasPressedThisFrame())
    {
        // 공격 입력은 즉시 실행하지 않고 다음 전투 처리 구간에서 소모한다.
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
```

</details>

## 2. 전투 진행과 스테이지 흐름

- 포탈 진입 후 전투 시작, 웨이브 스폰, 클리어 판정, 보상 선택, 다음 스테이지 이동이 이어지는 흐름을 구현했습니다.
- 웨이브 단위 몬스터 스폰과 스테이지 클리어 조건을 연결해 로그라이크 진행 구조를 만들었습니다.
- 마지막 스테이지에서는 보스전과 엔딩 UI가 이어지도록 구성했습니다.

파일:
- `Assets/01_Scripts/Features/Battle/Battle.cs`
- `Assets/01_Scripts/Features/Battle/BattleLoader.cs`
- `Assets/01_Scripts/Features/UI/GameClearPanel.cs`

<details>
  <summary><strong>코드 보기</strong></summary>

```csharp
public void StartBattle()
{
    // 아직 남은 스테이지가 있으면 다음 전투 로딩 코루틴을 시작한다.
    if (currentLevelIndex + 1 >= levels.Length) 
        StartCoroutine(LevelLoadingCoroutine());
}

IEnumerator SpawnCoroutine()
{
    currentMobIndex = -1;
    int id = 0;

    // 현재 스테이지에 포함된 웨이브를 순서대로 순회한다.
    while (++currentMobIndex < currentLevel.mobSetList.Count)
    {
        for (int i = 0; i < currentLevel.mobSetList[currentMobIndex].monsterList.Count; i++)
        {
            // 웨이브 내 몬스터를 일정 간격으로 스폰한다.
            yield return new WaitForSeconds(currentLevel.spawnDelay);

            currentMobCnt++;

            // 몬스터는 풀에서 꺼내고, 전투 상태와 위치를 초기화한다.
            Monster monster = PoolingManager.Instance.GetObject<Monster>(
                currentLevel.mobSetList[currentMobIndex].monsterList[i].mobName
            );

            monster.isBattle = true;
            monster.transform.position = GetSpawnPosition(monster.isGroundMob);
            monster.id = id++;
            monsterPos.Add(monster);
            monster.Setting();
        }
    }

    // 모든 웨이브 스폰이 끝났음을 표시한다.
    isSpawn = false;
}
```

</details>

## 3. 성장 및 보상 선택 기능

- 스테이지 클리어 후 3개의 보상 중 하나를 선택하는 구조를 구현했습니다.
- 아이템 획득 시 스탯 증가와 능력 효과 활성화가 동시에 반영되도록 구성했습니다.
- 전투 종료 후의 선택이 다음 전투 빌드 변화로 이어지도록 설계했습니다.

파일:
- `Assets/01_Scripts/Features/Item/Inventory.cs`
- `Assets/01_Scripts/Features/Item/SelectPanelGroup.cs`
- `Assets/01_Scripts/Features/Item/StatusData.cs`

<details>
  <summary><strong>코드 보기</strong></summary>

```csharp
public void GetItem(Item item)
{
    // 비어 있는 슬롯을 찾아 아이템을 장착한다.
    for (int i = 0; i < itemSlots.Length; i++)
    {
        if (itemSlots[i].SetItemUI(item))
        {
            // 아이템이 가진 스탯 보정을 현재 플레이어 데이터에 누적 적용한다.
            if (!item.statusData.Equals(baseData))
            {
                currentData.Apply(item.statusData);
                PlayerController.instance.StatusUpdate();
            }

            // activeEffectName과 연결된 런타임 효과를 활성화한다.
            if (item.activeEffectName != string.Empty)
            {
                for (int j = 0; j < activeEffects.Count; j++) 
                {
                    if (activeEffects[j].activeEffectName == item.activeEffectName)
                    {
                        activeEffects[j].Enable();

                        // 발동 방식에 따라 후속 처리 리스트에 등록한다.
                        if (activeEffects[j] is StackEffect stackEffect)
                            stackEffects.Add(stackEffect);
                        else if (activeEffects[j] is BattleEndEffect battleEndEffect)
                            battleEndEffects.Add(battleEndEffect);
                    }
                }
            }

            // 한 슬롯에 장착되면 처리를 종료한다.
            break;
        }
    }
}
```

</details>

## 4. 특수 능력 효과 구현

- 잔상 공격, 일반 투사체, 추적 투사체, 화살 보조 공격, 낙뢰, 연쇄 번개 등 전투 능력 효과를 구현했습니다.
- 능력 효과는 발동 방식에 따라 분리해, 새 효과를 추가할 수 있는 구조로 만들었습니다.
- 플레이어의 빌드 선택이 전투 방식 변화로 체감되도록 구성했습니다.

파일:
- `Assets/01_Scripts/Features/Item/EffectItemScript`

예시 기능:
- `DuelistsMirage`: 공격 후 지연 발동하는 잔상 공격
- `MagicFrostWand`: 일반 투사체 발사
- `TrackingFireWand`: 적 추적 투사체 발사
- `TripleBolt`: 확률형 낙뢰
- `BladeofShock`: 연쇄 번개

<details>
  <summary><strong>코드 보기</strong></summary>

```csharp
public class TripleBolt : StackEffect
{
    public override void Play()
    {
        // 확률이 충족되면 플레이어 전방에 3회의 낙뢰를 순차적으로 생성한다.
        if (Random.value <= hitChance)
        {
            for (int i = 1; i <= 3; i++)
            {
                Bolt thunder = PoolingManager.Instance.GetObject<Bolt>("Bolt");

                Vector3 pos = pc.transform.position;
                pos.y = height;
                pos.x += Random.Range(minWidth, maxWidth) * (pc.currentDir == Dir.Right ? 1 : -1);

                thunder.transform.position = pos;

                // 딜레이를 다르게 줘서 연속 낙뢰처럼 보이게 만든다.
                thunder.StartBolt(delay * i);
            }
        }
    }
}
```

</details>

## 5. 보스전 구현

- 최종 보스는 3페이즈 구조로 구현했습니다.
- 각 페이즈에서 여러 패턴 시퀀스를 배열로 구성해, 패턴 추가와 순서 변경이 가능하도록 만들었습니다.
- 페이즈 전환 시 체력 보정, 전용 연출, 배경 변화, 엔딩 UI가 함께 동작하도록 구성했습니다.

파일:
- `Assets/01_Scripts/Features/Monster/RedHood.cs`
- `Assets/01_Scripts/Features/UI/ScreenTransition.cs`
- `Assets/01_Scripts/Features/UI/GameClearPanel.cs`

<details>
  <summary><strong>코드 보기</strong></summary>

```csharp
// phase -> pattern -> action 순서로 보스 패턴 테이블을 구성한다.
patternActions = new Action[3][][];

patternActions[0] = new Action[3][];
patternActions[1] = new Action[3][];
patternActions[2] = new Action[3][];

// 페이즈별 패턴 길이를 먼저 선언하고,
patternActions[0][0] = new Action[5];
patternActions[1][0] = new Action[4];
patternActions[2][0] = new Action[5];

// 각 칸에 실제 보스 행동 함수를 매핑한다.
patternActions[0][0][0] = FastKnifeAttack;
patternActions[0][0][1] = ThrowPoisonApple;
patternActions[0][0][2] = KnifeAttack;

patternActions[2][1][0] = TripleTrackingArrow;
patternActions[2][1][1] = ChargeAxeAttack;
patternActions[2][1][3] = ChargeArrow;
```

</details>

## 기술 포인트

- ScriptableObject 기반으로 스테이지, 웨이브, 아이템 데이터를 분리해 콘텐츠 확장성을 확보했습니다.
- 상태 기반 플레이어 전투 구조로 입력, 이동, 공격, 피격, 스태미나를 일관된 흐름으로 관리했습니다.
- 오브젝트 풀링을 적용해 투사체와 이펙트가 많은 전투 상황을 안정적으로 처리했습니다.

## 데모

- [Web Demo](https://cksrb4309.github.io/Projects/scarlet_fall/index.html)

## 실행 환경

- Unity `6000.3.11f1`
- Scene
  - `Assets/02_Scenes/Title.unity`
  - `Assets/02_Scenes/Game.unity`
