using UnityEngine;

public class MagicFrostWand : TimerEffect
{
    public Transform startPos;
    public override void Play()
    {
        if (Battle.instance.monsterPos.Count > 0)
        {
            MagicProjectile projectile = PoolingManager.Instance.GetObject<MagicProjectile>
                ("MagicProjectile");

            projectile.transform.position = startPos.position;

            projectile.Setting();
        }
    }
}
