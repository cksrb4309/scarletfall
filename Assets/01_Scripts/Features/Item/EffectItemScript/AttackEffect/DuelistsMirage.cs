using UnityEngine;

public class DuelistsMirage : ActiveEffect
{
    public PlayerController pc;
    public MiragePlayerController mpc;

    public override void Enable()
    {
        mpc.gameObject.SetActive(true);

        pc.mpc = mpc;
    }
}