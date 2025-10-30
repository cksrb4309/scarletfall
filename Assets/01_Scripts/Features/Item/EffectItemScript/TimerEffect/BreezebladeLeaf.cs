using UnityEngine;

public class BreezebladeLeaf : TimerEffect
{
    public override void Play()
    {
        PlayerFlags.Value.Leaf = true;
    }
}
