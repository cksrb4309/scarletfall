using UnityEngine;

public class Maximizer : TimerEffect
{
    public MaximizerSupport support;
    public override void Play()
    {
        support.Enable();
    }
}