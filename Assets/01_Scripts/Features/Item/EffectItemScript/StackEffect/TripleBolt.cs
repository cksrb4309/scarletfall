using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TripleBolt : StackEffect
{
    public PlayerController pc;
    public float hitChance = 0.1f;
    float height = -0.07f;
    float minWidth = 0.8f;
    float maxWidth = 3f;
    float delay = 0.5f;

    public override void Play()
    {
        if (Random.value <= hitChance)
        {
            for (int i = 1; i <= 3; i++)
            {
                Bolt thunder = PoolingManager.Instance.GetObject<Bolt>("Bolt");

                Vector3 pos = pc.transform.position;

                pos.y = height;

                pos.x += Random.Range(minWidth, maxWidth) * (pc.currentDir == Dir.Right ? 1 : -1);

                thunder.transform.position = pos;

                thunder.StartBolt(delay * i);
            }
        }
    }
}
