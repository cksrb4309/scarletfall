using UnityEngine;

public class OnPlayerSprite : MonoBehaviour
{
    public Animator ar;

    public void StartMove()
    {
        ar.SetTrigger("Run");
    }
    public void StopMove()
    {
        ar.SetTrigger("Idle");
    }
}
