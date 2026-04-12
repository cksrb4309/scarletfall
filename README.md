# Scarletfall

`빨간 망토의 비극`은 10개 스테이지와 최종 보스전으로 구성된 2D 액션 로그라이크 프로젝트입니다.  
이 저장소는 게임 소개보다, 전투 시스템과 데이터 구조를 어떻게 구현했는지 보여주는 개발자 포트폴리오 용도로 정리했습니다.

## Tech Stack

`Unity` `C#` `ScriptableObject` `Input System` `URP`

## What I Implemented

- 스테이지 진입, 웨이브 스폰, 클리어, 보상 선택이 이어지는 전투 진행 구조
- ScriptableObject 기반 스테이지, 웨이브, 아이템 데이터 구조
- 보상 선택과 능력 효과 활성화를 연결한 성장 시스템
- 3페이즈 보스 패턴과 엔딩 UI까지 연결되는 최종 콘텐츠 흐름

## Implemented Systems

<details open>
  <summary><strong>1. Battle Flow</strong></summary>

  <br>

- `Battle`과 `BattleLoader`를 중심으로 스테이지 시작, 웨이브 스폰, 클리어 판정, 다음 전투 진입 흐름을 연결했습니다.
- 몬스터는 웨이브 단위로 스폰되며, 모든 적이 정리되면 클리어 연출과 보상 선택으로 이어집니다.

파일:
- `Assets/01_Scripts/Features/Battle/Battle.cs`
- `Assets/01_Scripts/Features/Battle/BattleLoader.cs`

코드 발췌:

```csharp
public void StartBattle()
{
    if (currentLevelIndex + 1 != levels.Length) 
        StartCoroutine(LevelLoadingCoroutine());
}

IEnumerator SpawnCoroutine()
{
    currentMobIndex = -1;
    int id = 0;

    while (++currentMobIndex < currentLevel.mobSetList.Count)
    {
        for (int i = 0; i < currentLevel.mobSetList[currentMobIndex].monsterList.Count; i++)
        {
            yield return new WaitForSeconds(currentLevel.spawnDelay);

            currentMobCnt++;

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

    isSpawn = false;
}
```

</details>

<details>
  <summary><strong>2. Data-Driven Content</strong></summary>

  <br>

- 스테이지, 웨이브, 아이템, 확률 데이터를 ScriptableObject로 분리했습니다.
- 코드 수정 없이 데이터 교체만으로 전투 구성과 보상 밸런싱을 조정할 수 있게 설계했습니다.

파일:
- `Assets/01_Scripts/Features/Battle/LevelSetting.cs`
- `Assets/01_Scripts/Features/Battle/MobSetting.cs`
- `Assets/01_Scripts/Features/Item/Item.cs`
- `Assets/01_Scripts/Features/Item/StatusData.cs`

코드 발췌:

```csharp
[CreateAssetMenu(fileName = "LevelSetting", menuName = "Scriptable Objects/LevelSetting")]
public class LevelSetting : ScriptableObject
{
    public List<MobSetting> mobSetList;
    public ItemProbability itemProbability;
    public float nextWaveDelay;
    public float spawnDelay;
}

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public Sprite icon;
    [TextArea] public string description;
    public ItemGrade itemGrade;
    public StatusData statusData;
    public string activeEffectName = string.Empty;
}
```

</details>

<details>
  <summary><strong>3. Growth System</strong></summary>

  <br>

- 전투 종료 후 3개의 보상 중 하나를 선택하는 구조를 구현했습니다.
- 아이템 획득 시 스탯 누적과 능력 효과 활성화가 동시에 반영되도록 구성했습니다.
- 능력 효과는 `ActiveEffect`, `StackEffect`, `BattleEndEffect`, `TimerEffect` 계층으로 분리해 확장 가능하게 만들었습니다.

파일:
- `Assets/01_Scripts/Features/Item/Inventory.cs`
- `Assets/01_Scripts/Features/Item/SelectPanelGroup.cs`
- `Assets/01_Scripts/Features/Item/EffectItemScript`

코드 발췌:

```csharp
public void GetItem(Item item)
{
    for (int i = 0; i < itemSlots.Length; i++)
    {
        if (itemSlots[i].SetItemUI(item))
        {
            if (!item.statusData.Equals(baseData))
            {
                currentData.Apply(item.statusData);
                PlayerController.instance.StatusUpdate();
            }

            if (item.activeEffectName != string.Empty)
            {
                for (int j = 0; j < activeEffects.Count; j++) 
                {
                    if (activeEffects[j].activeEffectName == item.activeEffectName)
                    {
                        activeEffects[j].Enable();

                        if (activeEffects[j] is StackEffect stackEffect)
                            stackEffects.Add(stackEffect);
                        else if (activeEffects[j] is BattleEndEffect battleEndEffect)
                            battleEndEffects.Add(battleEndEffect);
                    }
                }
            }
            break;
        }
    }
}
```

</details>

<details>
  <summary><strong>4. Boss AI</strong></summary>

  <br>

- 최종 보스는 3페이즈 구조로 구현했습니다.
- 각 페이즈에서 여러 패턴 시퀀스를 `Action` 배열로 구성해, 패턴 추가와 순서 변경이 가능하도록 만들었습니다.
- 페이즈 전환 시 체력 보정, 연출, 패턴 재선택이 함께 동작하도록 구현했습니다.

파일:
- `Assets/01_Scripts/Features/Monster/RedHood.cs`

코드 발췌:

```csharp
patternActions = new Action[3][][];

patternActions[0] = new Action[3][];
patternActions[1] = new Action[3][];
patternActions[2] = new Action[3][];

patternActions[0][0] = new Action[5];
patternActions[1][0] = new Action[4];
patternActions[2][0] = new Action[5];

patternActions[0][0][0] = FastKnifeAttack;
patternActions[0][0][1] = ThrowPoisonApple;
patternActions[0][0][2] = KnifeAttack;

patternActions[2][1][0] = TripleTrackingArrow;
patternActions[2][1][1] = ChargeAxeAttack;
patternActions[2][1][3] = ChargeArrow;
```

</details>

## Technical Points

<details>
  <summary><strong>State-Based Player Combat</strong></summary>

  <br>

- 입력, 애니메이션, 이동, 스태미나, 피격 처리를 플레이어 상태 기준으로 묶었습니다.
- 지상 콤보, 공중 공격, 구르기, 급강하 공격이 상태 전이 안에서 동작하도록 구성했습니다.

```csharp
if (jumpInputAction.action.WasPressedThisFrame())
{
    Jump();
}
else if (attackInputAction.action.WasPressedThisFrame())
{
    if (PlayerFlags.Value.SwingCheck == false)
        PlayerFlags.Value.SwingCheck = true;
}
else if (rollInputAction.action.WasPressedThisFrame())
{
    Roll();
}
```

</details>

<details>
  <summary><strong>Object Pooling</strong></summary>

  <br>

- 투사체, 히트 이펙트, 회복 오브젝트 등 반복 생성되는 객체를 풀링으로 관리했습니다.
- 전투 중 빈번하게 생성되는 오브젝트를 재사용하도록 구성했습니다.

```csharp
public T GetObject<T>(string poolName) where T : Component
{
    if (pools.ContainsKey(poolName))
    {
        return pools[poolName].GetObject<T>();
    }
    return null;
}

public void ReturnObject(string poolName, GameObject obj)
{
    if (pools.ContainsKey(poolName))
        pools[poolName].ReturnObject(obj);
    else
        Destroy(obj);
}
```

</details>

<details>
  <summary><strong>UI / Transition Utilities</strong></summary>

  <br>

- 화면 전환과 스테이지 연출은 별도 전환 시스템으로 분리했습니다.
- UI 이동과 일부 지연 호출에는 DOTween을 보조적으로 사용했습니다.

```csharp
ScreenTransition.Play(new ScreenTransitionOptions
{
    StartTransitionName = "Leaf_FadeOut",
    EndTransitionName = "Leaf_FadeIn",
    FadeStart = 0f,
    FadeEnd = 0f,
    FadeDuration = 2f,
    OnTransitionComplete = () =>
    {
        ShowLobby();
        DOVirtual.DelayedCall(5f, () => Inventory.instance.selectPanelGroup.StartSelectItem());
    }
});
```

</details>

## Demo

- [Move Tutorial](./Assets/11_Video/MoveTutorial.mp4)
- [Attack Tutorial](./Assets/11_Video/AttackTutorial.mp4)
- [Roll Tutorial](./Assets/11_Video/RollTutorial.mp4)

## How to Run

- Unity Version: `6000.0.20f1`
- Scenes:
  - `Assets/02_Scenes/Title.unity`
  - `Assets/02_Scenes/Game.unity`
