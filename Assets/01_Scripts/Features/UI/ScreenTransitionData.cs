using Coffee.UIExtensions;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "ScreenTransitionData", menuName = "UI Effect/ScreenTransitionData")]
public class ScreenTransitionData : ScriptableObject
{
    public string Name;
    public Color FadeImageColor;
    public Color TransitionColor;
    public UIParticle Particle;
    public Material Material;
    public float Length;
}
