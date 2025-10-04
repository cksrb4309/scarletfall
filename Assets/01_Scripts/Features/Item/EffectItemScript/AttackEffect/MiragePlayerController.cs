using System.Collections;
using UnityEngine;

public class MiragePlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator ar;
    public float delay = 0.5f;
    public float lastJumpSwing;
    public void SetAnimation(PlayerState ps, Vector3 pos, Dir dir)
    {
        StartCoroutine(ApplyCoroutine(ps, pos, dir));
    }

    IEnumerator ApplyCoroutine(PlayerState ps, Vector3 pos, Dir dir)
    {
        yield return new WaitForSeconds(delay);
        switch (ps)
        {
            case PlayerState.Swing1: ar.SetTrigger("Swing1"); break;
            case PlayerState.Swing2: ar.SetTrigger("Swing2"); break;
            case PlayerState.Swing3: ar.SetTrigger("Swing3"); break;

            case PlayerState.JumpSwing1: ar.SetTrigger("JumpSwing1"); break;
            case PlayerState.JumpSwing2: ar.SetTrigger("JumpSwing2"); break;
            case PlayerState.JumpSwing3: ar.SetTrigger("JumpSwing3"); break;
        }
        transform.position = pos;
        transform.localScale = new Vector3(dir == Dir.Right ? 1 : -1, 1, 1);
    }
    void LastJumpSwing()
    {
        rb.linearVelocityY = lastJumpSwing;
    }
    public void TouchGround()
    {
        rb.linearVelocityY = 0;
        
        ar.SetTrigger("Idle");
    }
    public void Swing(int number)
    {
        SoundManager.Play("PlayerAttack" + number.ToString(), SoundType.Effect);
    }
}
