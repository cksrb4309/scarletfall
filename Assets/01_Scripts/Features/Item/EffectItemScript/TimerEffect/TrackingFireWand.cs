using UnityEngine;

public class TrackingFireWand : TimerEffect
{
    public Transform startPos;
    public override void Play()
    {
        if (Battle.instance.monsterPos.Count > 0)
        {
            GameObject projectile = PoolingManager.Instance.GetObject("TrackingProjectile");

            projectile.transform.position = startPos.position;
        }
    }
}