using UnityEngine;

public class AudioSelectPlay : MonoBehaviour
{
    public void OnUIEnterSound()
    {
        SoundManager.Play("UI_Enter", SoundType.Effect);
    }
    public void OnUIClickSound()
    {
        SoundManager.Play("UI_Click", SoundType.Effect);
    }
}
