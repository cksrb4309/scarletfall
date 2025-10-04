using UnityEngine;

public class TitlePlayerController : MonoBehaviour
{
    private Animator myAnimator;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        myAnimator.SetTrigger("Landing");
    }   
}
