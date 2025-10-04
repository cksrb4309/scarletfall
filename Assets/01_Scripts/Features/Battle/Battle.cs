using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    public static Battle instance;

    public CurrentStage currentStage;

    public SelectPanelGroup selectPanelGroup;

    public LevelSetting[] levels;

    public Transform[] groundSpawnPoints;

    public Transform[] airSpawnPoints;

    public Transform bossSpawnPoint;

    public float battleStartDelay;

    public List<Monster> monsterPos;

    public BattleLoader battleLoader;

    bool[] airSpawnPointsVisited;
    bool[] groundSpawnPointsVisited;

    LevelSetting currentLevel;

    int currentLevelIndex = -1;
    int currentMobIndex = 0;
    int currentMobCnt = 0;

    bool isSpawn = false;

    void Start()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        groundSpawnPointsVisited = new bool[groundSpawnPoints.Length];
        airSpawnPointsVisited = new bool[airSpawnPoints.Length];
    }

    public void StartBattle()
    {
        if (currentLevelIndex + 1 != levels.Length) 
        {
            StartCoroutine(LevelLoadingCoroutine());
        }
    }
    IEnumerator LevelLoadingCoroutine()
    {
        isSpawn = true;
        yield return new WaitForSeconds(battleStartDelay);

        currentLevel = levels[++currentLevelIndex];

        selectPanelGroup.p = currentLevel.itemProbability;

        StartCoroutine(SpawnCoroutine());
    }
    IEnumerator SpawnCoroutine()
    {
        currentMobIndex = -1;
        int id = 0;
        while (++currentMobIndex < currentLevel.mobSetList.Count)
        {
            for (int i = 0; i < groundSpawnPointsVisited.Length; i++) groundSpawnPointsVisited[i] = false;
            for (int i = 0; i < airSpawnPointsVisited.Length; i++) airSpawnPointsVisited[i] = false;


            for (int i = 0; i < currentLevel.mobSetList[currentMobIndex].monsterList.Count; i++)
            {
                yield return new WaitForSeconds(currentLevel.spawnDelay); // 몹 한마리 당 스폰 딜레이 적용

                currentMobCnt++;

                Monster monster = PoolingManager.Instance.GetObject<Monster>(currentLevel.mobSetList[currentMobIndex].monsterList[i].mobName);

                monster.isBattle = true;

                Vector3 pos = GetSpawnPosition(monster.isGroundMob);

                if (monster.mobName.Equals("RedHood")) pos = bossSpawnPoint.position;

                monster.transform.position = pos;

                monster.id = id++;

                monsterPos.Add(monster);

                monster.Setting();
            }

            if (currentMobIndex < currentLevel.mobSetList.Count - 1)
                yield return new WaitForSeconds(currentLevel.nextWaveDelay);
        }
        // 스폰 종료 시점
        isSpawn = false;
    }

    Vector3 GetSpawnPosition(bool isGroundMob)
    {
        int spawnPoint = -1;

        while (spawnPoint == -1)
        {
            int randomIndex = Random.Range(0, isGroundMob ? groundSpawnPoints.Length : airSpawnPoints.Length);

            if (!(isGroundMob ? groundSpawnPointsVisited : airSpawnPointsVisited)[randomIndex]) 
            {
                (isGroundMob ? groundSpawnPointsVisited : airSpawnPointsVisited)[randomIndex] = true;

                return (isGroundMob ? groundSpawnPoints : airSpawnPoints)[randomIndex].position;
            }
        }

        Debug.LogWarning("GetSpawnPosition 확인 바람");
        return Vector3.one * 1000f;
    }

    public void KillMonster()
    {
        if (!PlayerController.instance.IsAlive) return;
        if (currentLevelIndex != levels.Length - 1)
        {
            if (--currentMobCnt == 0 && isSpawn == false)
            {
                battleLoader.ClearLoad();
            }
        }
    }
    public void RemoveMonsterTransform(int id)
    {
        for (int i = 0; i < monsterPos.Count; i++)
        {
            if (monsterPos[i].id == id)
            {
                // 해당 Transform을 리스트에서 제거
                monsterPos.RemoveAt(i);

                // 반복 탈출
                break;
            }
        }
    }
    public void EnableMonster()
    {
        currentMobCnt++;
    }
}