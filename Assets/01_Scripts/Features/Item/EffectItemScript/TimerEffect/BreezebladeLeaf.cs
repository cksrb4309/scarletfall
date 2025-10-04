using UnityEngine;

public class BreezebladeLeaf : TimerEffect
{
    public PlayerController pc;

    public override void Play()
    {
        pc.leaf = true;
    }
}
