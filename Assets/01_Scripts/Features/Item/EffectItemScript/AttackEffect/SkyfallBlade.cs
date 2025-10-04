using UnityEngine;

public class SkyfallBlade : ActiveEffect
{
    public Transform startPos;

    public PlayerController pc;
    public float speed;
    public float destroyDelay = 0.3f;
    bool isEnable = false;
    public override void Enable()
    {
        isEnable = true;
    }

    public override void Play()
    {
        if (!isEnable) return;

        ShockWave shockWave = PoolingManager.Instance.GetObject<ShockWave>("ShockWave");

        Debug.Log("ShockWave:" + shockWave.gameObject.name);

        shockWave.transform.localScale = new Vector3(pc.currentDir == Dir.Right ? 1 : -1, 2, 1);

        shockWave.transform.position = new Vector3(startPos.position.x, -2.769f, 0);

        shockWave.speed = speed;

        shockWave.dir = new Vector3(pc.currentDir == Dir.Right ? 1 : -1, 0, 0);

        shockWave.destroyDelay = destroyDelay;

        shockWave.CallDestroy();
    }
}
