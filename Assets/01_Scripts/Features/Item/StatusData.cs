using UnityEngine;

[CreateAssetMenu(fileName = "StatusData", menuName = "Scriptable Objects/StatusData")]
public class StatusData : ScriptableObject
{
    public float playerDamage;           // 플레이어 데미지 %
    public float playerHp;              // 플레이어 체력 %
    public float playerStaminaRegen;     // 플레이어 스태미나 회복 속도 %
    public float playerMoveSpeed;        // 플레이어 이동 속도 %
    public float rollRange;             // 플레이어 구르기 거리 %

    public float playerCriticalChance;     // 플레이어 치명타 확률 +
    public float playerAvoidChance;        // 플레이어 회피 확률 +

    public float monsterDamage;          // 몬스터 데미지 %
    public float monsterHp;              // 몬스터 체력 %


    public float monsterCriticalChance;    // 몬스터 치명타 확률 +
    public float monsterAvoidChance;       // 몬스터 회피 확률 +

    public void Init()
    {
        playerDamage = 1.0f;
        playerHp = 1.0f;
        playerStaminaRegen = 1.0f;
        playerMoveSpeed = 1.0f;
        rollRange = 1.0f;

        playerCriticalChance = 0f;
        playerAvoidChance = 0f;

        monsterDamage = 1.0f;
        monsterHp = 1.0f;

        monsterCriticalChance = 0f;
        monsterAvoidChance = 0f;
    }
    public void Apply(StatusData other)
    {
        StatusData baseData = new StatusData();
        baseData.Init();

        playerDamage += other.playerDamage - baseData.playerDamage;
        playerHp += other.playerHp - baseData.playerHp;
        Debug.Log("OtherHP:" + other.playerHp.ToString() + "/BaseHP:" + baseData.playerHp.ToString());
        Debug.Log("PlayerHP:" + playerHp.ToString());
        playerStaminaRegen += other.playerStaminaRegen - baseData.playerStaminaRegen;
        playerMoveSpeed += other.playerMoveSpeed - baseData.playerMoveSpeed;
        playerAvoidChance += other.playerAvoidChance - baseData.playerAvoidChance;
        playerCriticalChance += other.playerCriticalChance - baseData.playerCriticalChance;

        rollRange += other.rollRange - baseData.rollRange;
        monsterAvoidChance += other.monsterAvoidChance - baseData.monsterAvoidChance;
        monsterDamage += other.monsterDamage - baseData.monsterDamage;
        monsterHp += other.monsterHp - baseData.monsterHp;
        monsterCriticalChance += other.monsterCriticalChance - baseData.monsterCriticalChance;
    }
}