using UnityEngine;

public class StackEffect : ActiveEffect
{
    public int needStackCount;
    public int currentCount;

    public virtual void FillCount()
    {
        currentCount++;

        Debug.Log("currentCount:" + currentCount.ToString());

        if (currentCount >= needStackCount)
        {
            currentCount = 0;

            Play();

            Debug.Log("Play :" + activeEffectName);
        }
    }
    public override void Play()
    {

    }
}