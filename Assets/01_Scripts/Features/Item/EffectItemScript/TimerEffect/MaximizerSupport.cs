using UnityEngine;

public class MaximizerSupport : MonoBehaviour
{
    public static MaximizerSupport instance;
    public Animator ar;
    private void Start()
    {
        instance = this;
    }
    public void Enable()
    {
        ar.SetTrigger("On");

        SoundManager.Play("MaximizerCharge", SoundType.Effect);
    }
    public void OnMaximizer()
    {
        PlayerController.isMaximizer = true;
    }
    public void OffMaximizer()
    {
        Debug.Log("ºÒ¸²");

        ar.SetTrigger("Off");
    }
}