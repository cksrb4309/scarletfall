public class PlayerStat
{
    public static PlayerStat Value
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerStat();

                _instance.SetDefaultStat();
                _instance.SetDifficultyStat();
            }
            else if (Option.difficulty != _instance.selectDifficulty)
            {
                _instance.SetDifficultyStat();
            }

            return _instance;
        }
    }

    private static PlayerStat _instance = null;

    private Difficulty selectDifficulty = Difficulty.Easy;

    // 난이도에 따른 변화 스탯
    public float MoveSpeed = 350;
    public float JumpSpeed = 13.5f;
    public float PlayerMaxHpBase = 15000;
    public float PlayerMaxSp = 120;
    public float RollUseStamina = 15;
    public float JumpUseStamina = 0;
    public float StaminaRegenSpeed = 12;

    // 고정 스탯
    public float LastJumpSwing = -15;
    public float RollRange = 4;
    public float RollSpeed = 2.3f;
    public float SwingUseStamina = 15;

    private void SetDefaultStat()
    {
        LastJumpSwing = -15;
        RollRange = 4;
        RollSpeed = 2.3f;
        SwingUseStamina = 15;
    }
    private void SetDifficultyStat()
    {
        switch (Option.difficulty)
        {
            case Difficulty.Easy:
                MoveSpeed = 350;
                JumpSpeed = 13.5f;
                PlayerMaxHpBase = 15000;
                PlayerMaxSp = 120;
                RollUseStamina = 15;
                JumpUseStamina = 0;
                StaminaRegenSpeed = 12;
                break;

            case Difficulty.Normal:
                MoveSpeed = 300;
                JumpSpeed = 13;
                PlayerMaxHpBase = 12500;
                PlayerMaxSp = 100;
                RollUseStamina = 20;
                JumpUseStamina = 3;
                StaminaRegenSpeed = 10;
                break;

            case Difficulty.Hard:
                MoveSpeed = 270;
                JumpSpeed = 12.5f;
                PlayerMaxHpBase = 10000;
                PlayerMaxSp = 80;
                RollUseStamina = 30;
                JumpUseStamina = 5;
                StaminaRegenSpeed = 8;
                break;
        }
    }
}