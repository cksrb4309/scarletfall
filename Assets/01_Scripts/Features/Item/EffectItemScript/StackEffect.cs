using UnityEngine;

public class StackEffect : ActiveEffect
{
    public int needStackCount;
    public int currentCount;

    public virtual void FillCount()
    {
        currentCount++;

        if (currentCount >= needStackCount)
        {
            currentCount = 0;

            Play();
        }
    }
    public override void Play()
    {

    }
}