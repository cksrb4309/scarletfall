using UnityEngine;

public class BowofOpportunity : ActiveEffect
{
    public Bow[] bow;
    public override void Enable()
    {
        for (int i = 0; i < bow.Length; i++)
        {
            bow[i].enabled = true;
        }
    }
}
