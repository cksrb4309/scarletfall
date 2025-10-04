using Unity.VisualScripting;
using UnityEngine;

public class BloodletterBlade : ActiveEffect
{
    public override void Enable()
    {
        PlayerController.isLifesteal = true;
    }
}
