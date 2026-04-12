using UnityEngine;

public class PlayerFlags
{
    public static PlayerFlags Value
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerFlags();
            }

            return _instance;
        }
    }

    private static PlayerFlags _instance = null;

    public int SwingCombo = 0;
    public int JumpSwingCombo = 0;

    public bool Invincibility       = false; // true일 시 무적
    public bool SwingCheck          = false;
    public bool IsStop              = false;
    public bool IsAttacking         = false;
    public bool JumpUse             = false;
    public bool Jumping             = false;
    public bool Rolling             = false;
    public bool DieCheck            = false;
    public bool FastDownAttackCheck = false;
    public bool FastDownAttacking   = false;
    public bool IsLifesteal         = false;
    public bool IsMaximizer         = false;
    public bool TriggerLocked       = false;
    public bool Leaf                = false;       // 아이템으로 인해 스태미나 소모가 필요없을 때
    public bool Shield              = false;      // 아이템으로 인해 공격을 막을 수 있을 때

    public void ResetFlags()
    {
        SwingCombo = 0;
        JumpSwingCombo = 0;
        Invincibility = false;
        SwingCheck = false;
        IsStop = false;
        IsAttacking = false;
        JumpUse = false;
        Jumping = false;
        Rolling = false;
        DieCheck = false;
        FastDownAttackCheck = false;
        FastDownAttacking = false;
        IsLifesteal = false;
        IsMaximizer = false;
        TriggerLocked = false;
        Leaf = false;
        Shield = false;
    }
}