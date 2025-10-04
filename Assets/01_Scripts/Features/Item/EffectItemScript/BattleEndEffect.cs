using UnityEngine;

public class BattleEndEffect : ActiveEffect
{
    public bool isEnable = false;

    public override void Enable()
    {
        isEnable = true;
    }
}