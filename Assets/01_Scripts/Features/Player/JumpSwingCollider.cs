using UnityEngine;

public class JumpSwingCollider : MonoBehaviour
{
    public PlayerController pc;
    public MiragePlayerController mpc;
    public SkyfallBlade skyfallBlade;

    bool isAttack = true;

    private void OnEnable()
    {
        isAttack = true;
    }
    private void OnDisable()
    {
        isAttack = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pc != null)
        {
            if (isAttack)
            {
                Debug.Log("»Æ¿Œ 321");

                pc.TouchGround();

                skyfallBlade.Play();

                isAttack = false;
            }
        }
        else
        {
            mpc.TouchGround();
        }
    }
}
