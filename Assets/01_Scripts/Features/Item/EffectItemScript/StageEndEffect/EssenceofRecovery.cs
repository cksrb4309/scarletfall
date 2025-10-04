using UnityEngine;

public class EssenceofRecovery : BattleEndEffect
{
    public PlayerController pc;
    public float heal;
    public override void Play()
    {
        pc.Hit(-heal);
    }
}