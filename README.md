# Scarletfall

`빨간 망토의 비극`은 10개 스테이지와 최종 보스전으로 구성된 2D 액션 로그라이크 프로젝트입니다.  
플레이어는 스테이지를 돌파하며 아이템을 선택하고, 능력 효과와 스탯 성장을 조합해 전투 빌드를 완성합니다.

## 프로젝트 개요

- 장르: 2D 액션 로그라이크
- 엔진: Unity 6
- 주요 기술: C#, ScriptableObject, URP, Input System, DOTween
- 핵심 목표: 전투 루프, 성장 시스템, 보스전 연출이 하나의 플레이 흐름으로 자연스럽게 이어지는 액션 로그라이크 구현

## 핵심 구현

### 1. 전투 루프 구현

- 포탈 진입 -> 전투 시작 -> 웨이브 스폰 -> 스테이지 클리어 -> 보상 선택 -> 다음 스테이지 진입의 흐름을 구현했습니다.
- 마지막 스테이지에서는 최종 보스전과 엔딩 UI가 연결되도록 구성했습니다.

관련 코드:
- `Assets/01_Scripts/Features/Battle/Battle.cs`
- `Assets/01_Scripts/Features/Battle/BattleLoader.cs`
- `Assets/01_Scripts/Features/UI/GameClearPanel.cs`

### 2. 데이터 중심 콘텐츠 구조 설계

- 스테이지, 웨이브, 몬스터, 아이템, 확률 데이터를 ScriptableObject로 분리했습니다.
- 코드 수정 없이 데이터 교체만으로 스테이지 구성과 아이템 밸런싱을 조정할 수 있도록 설계했습니다.

관련 코드:
- `Assets/01_Scripts/Features/Battle/LevelSetting.cs`
- `Assets/01_Scripts/Features/Battle/MobSetting.cs`
- `Assets/01_Scripts/Features/Item/Item.cs`
- `Assets/01_Scripts/Features/Item/ItemProbability.cs`
- `Assets/01_Scripts/Features/Item/StatusData.cs`

관련 데이터:
- `Assets/06_ScriptableObject/LevelSetting`
- `Assets/06_ScriptableObject/MobSettings`
- `Assets/06_ScriptableObject/Item`

### 3. 성장 시스템 구현

- 스테이지 클리어 후 3개의 보상 중 하나를 선택하는 구조를 구현했습니다.
- 아이템 획득 시 스탯 증가와 능력 효과 활성화가 동시에 반영되도록 설계했습니다.
- 공격 트리거형, 주기 발동형, 누적 타격형, 전투 종료형 효과를 분리해 확장 가능한 구조로 구성했습니다.

대표 능력 예시:
- 공격 후 지연 발동하는 잔상 공격
- 일반 에너지 투사체
- 적을 추적하는 투사체
- 전방 화살 발사
- 확률형 낙뢰
- 누적 타격 기반 연쇄 번개

관련 코드:
- `Assets/01_Scripts/Features/Item/Inventory.cs`
- `Assets/01_Scripts/Features/Item/SelectPanelGroup.cs`
- `Assets/01_Scripts/Features/Item/EffectItemScript`

### 4. 플레이어 전투와 보스전 완성

- 플레이어 전투에 콤보 공격, 공중 공격, 구르기, 스태미나, 급강하 공격, 피격 판정을 통합했습니다.
- 최종 보스는 3페이즈 패턴 구조로 구현했으며, 패턴 전환과 전용 연출이 이어지도록 구성했습니다.
- 보스전 전용 배경 전환과 엔딩 UI를 통해 게임의 마무리 경험을 완성했습니다.

관련 코드:
- `Assets/01_Scripts/Features/Player/PlayerController.cs`
- `Assets/01_Scripts/Features/Monster/Monster.cs`
- `Assets/01_Scripts/Features/Monster/RedHood.cs`
- `Assets/01_Scripts/Features/UI/ScreenTransition.cs`

## 기술 포인트

### 전투 시스템

- 상태 기반 플레이어 전투 구조
- 입력, 애니메이션, 이동, 피격, 스태미나를 하나의 전투 흐름으로 연결
- 일반 몬스터와 보스 패턴을 분리해 콘텐츠 난이도와 전투 리듬을 조절

### 성장 시스템

- 선택형 보상 구조를 통해 전투 이후의 의사결정을 플레이 경험에 반영
- 스탯 증가와 능력 효과를 분리해 밸런싱과 확장을 쉽게 설계
- 전투와 성장의 연결점을 명확하게 만들어 로그라이크 구조를 강화

### 전투 지원 시스템

- Input System 기반 입력 매핑 관리
- DOTween 및 화면 전환 연출을 활용한 스테이지 흐름 구성
- 오브젝트 풀링을 적용해 투사체와 이펙트 중심 전투를 안정적으로 처리

관련 코드:
- `Assets/01_Scripts/Core/Manager/InputManager.cs`
- `Assets/01_Scripts/Core/Manager/PoolingManager.cs`
- `Assets/01_Scripts/Core/Manager/ObjectPool.cs`
- `Assets/01_Scripts/Core/Manager/SoundManager.cs`

## 확인 가능한 콘텐츠 범위

- 스테이지 데이터: 일반 스테이지 + 최종 보스전 데이터 구성
- 몬스터: Slime, Goblin, FlyingEye, RedHood
- 아이템 데이터: 다수의 등급별 아이템 데이터와 활성 효과 구성
- 튜토리얼 영상 리소스 포함

데모 파일:
- [Move Tutorial](./Assets/11_Video/MoveTutorial.mp4)
- [Attack Tutorial](./Assets/11_Video/AttackTutorial.mp4)
- [Roll Tutorial](./Assets/11_Video/RollTutorial.mp4)
- [Jump Tutorial](./Assets/11_Video/JumpTutorial.mp4)
- [Land Attack Tutorial](./Assets/11_Video/LandAttackTutorial.mp4)
- [Interaction Tutorial](./Assets/11_Video/InteractionTutorial.mp4)

## 프로젝트 구조

```text
Assets
├─ 01_Scripts
│  ├─ Core
│  └─ Features
│     ├─ Battle
│     ├─ Item
│     ├─ Monster
│     ├─ Player
│     └─ UI
├─ 02_Scenes
├─ 04_Prefab
├─ 06_ScriptableObject
└─ 11_Video
```

## 실행 환경

- Unity Editor Version: `6000.0.20f1`
- Build Scenes:
  - `Assets/02_Scenes/Title.unity`
  - `Assets/02_Scenes/Game.unity`

## 요약

이 프로젝트는 액션 로그라이크의 핵심인 `전투`, `성장`, `보스전`, `연출 흐름`을 하나의 플레이 경험으로 연결하는 데 집중했습니다.  
포트폴리오 관점에서는 다음 내용을 중심으로 볼 수 있습니다.

- 전투 루프를 끝까지 완성한 구조 설계
- ScriptableObject 기반 데이터 중심 콘텐츠 구성
- 보상 선택과 능력 효과가 결합된 성장 시스템
- 3페이즈 보스전과 엔딩으로 이어지는 최종 콘텐츠 완성
