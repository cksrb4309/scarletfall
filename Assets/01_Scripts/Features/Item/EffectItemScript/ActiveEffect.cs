using UnityEngine;

public class ActiveEffect : MonoBehaviour
{
    public string activeEffectName;

    public virtual void Play() { }
    public virtual void Enable() { }
}