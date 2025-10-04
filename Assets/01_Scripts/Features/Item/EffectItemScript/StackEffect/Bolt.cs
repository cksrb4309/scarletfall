using System.Collections;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    public Animator ar;

    public void StartBolt(float delay)
    {
        StartCoroutine(StartBoltCoroutine(delay));
    }
    IEnumerator StartBoltCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        ar.SetTrigger("On");
    }
    public void ReturnObj()
    {
        PoolingManager.Instance.ReturnObject("Bolt", gameObject);
    }
    void CreateBoltSoundPlay()
    {
        SoundManager.Play("CreateBolt", SoundType.Effect);
    }
}
