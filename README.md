# Scarletfall

`빨간 망토의 비극`은 10개 스테이지와 최종 보스전으로 구성된 2D 액션 로그라이크 프로젝트입니다.  
전투 루프, 성장 시스템, 보스전 패턴 구현을 중심으로 제작했습니다.

## Tech Stack

`Unity` `C#` `ScriptableObject` `Input System` `DOTween`

## 핵심 구현

- 10개 스테이지와 최종 보스전으로 이어지는 액션 로그라이크 전투 루프 구현
- ScriptableObject 기반 스테이지, 웨이브, 아이템 데이터 구조 설계
- 보상 선택과 능력 효과가 결합된 성장 시스템 구현
- 3페이즈 보스 패턴과 엔딩 UI를 포함한 최종 콘텐츠 흐름 완성

<details>
  <summary><strong>자세히 보기</strong></summary>

  <br>

### 전투 루프

- 포탈 진입 -> 전투 시작 -> 웨이브 스폰 -> 스테이지 클리어 -> 보상 선택 -> 다음 스테이지 진입 흐름 구현
- 최종 스테이지에서 보스전과 엔딩 UI 연결

관련 코드:
- `Assets/01_Scripts/Features/Battle/Battle.cs`
- `Assets/01_Scripts/Features/Battle/BattleLoader.cs`

### 데이터 구조

- 스테이지, 웨이브, 몬스터, 아이템, 확률 데이터를 ScriptableObject로 분리
- 데이터 교체만으로 전투 구성과 밸런싱이 가능하도록 설계

관련 코드:
- `Assets/01_Scripts/Features/Battle/LevelSetting.cs`
- `Assets/01_Scripts/Features/Item/Item.cs`
- `Assets/01_Scripts/Features/Item/StatusData.cs`

### 성장 시스템

- 스테이지 클리어 후 3선택 보상 구조 구현
- 스탯 증가와 능력 효과 활성화가 다음 전투 체감에 반영되도록 설계

관련 코드:
- `Assets/01_Scripts/Features/Item/Inventory.cs`
- `Assets/01_Scripts/Features/Item/SelectPanelGroup.cs`

### 보스전

- 3페이즈 패턴과 전용 연출, 배경 변화, 엔딩 UI 구성
- 일반 스테이지와 구분되는 최종전 흐름 완성

관련 코드:
- `Assets/01_Scripts/Features/Monster/RedHood.cs`
- `Assets/01_Scripts/Features/UI/GameClearPanel.cs`

</details>

## 기술 포인트

<details>
  <summary><strong>전투 시스템</strong></summary>

  <br>

- 상태 기반 플레이어 전투 구조
- 입력, 애니메이션, 이동, 피격, 스태미나를 하나의 전투 흐름으로 연결
- 일반 몬스터와 보스 패턴을 분리해 콘텐츠 난이도와 전투 리듬을 조절

</details>

<details>
  <summary><strong>성장 시스템</strong></summary>

  <br>

- 선택형 보상 구조를 통해 전투 이후의 의사결정을 플레이 경험에 반영
- 스탯 증가와 능력 효과를 분리해 밸런싱과 확장을 쉽게 설계
- 전투와 성장의 연결점을 명확하게 만들어 로그라이크 구조를 강화

</details>

<details>
  <summary><strong>전투 지원 시스템</strong></summary>

  <br>

- Input System 기반 입력 매핑 관리
- DOTween 및 화면 전환 연출을 활용한 스테이지 흐름 구성
- 오브젝트 풀링을 적용해 투사체와 이펙트 중심 전투를 안정적으로 처리

관련 코드:
- `Assets/01_Scripts/Core/Manager/InputManager.cs`
- `Assets/01_Scripts/Core/Manager/PoolingManager.cs`
- `Assets/01_Scripts/Core/Manager/ObjectPool.cs`
- `Assets/01_Scripts/Core/Manager/SoundManager.cs`

</details>

## Demo

- [Move Tutorial](./Assets/11_Video/MoveTutorial.mp4)
- [Attack Tutorial](./Assets/11_Video/AttackTutorial.mp4)
- [Roll Tutorial](./Assets/11_Video/RollTutorial.mp4)

## 실행 환경

- Unity `6000.0.20f1`
- Scene
  - `Assets/02_Scenes/Title.unity`
  - `Assets/02_Scenes/Game.unity`
