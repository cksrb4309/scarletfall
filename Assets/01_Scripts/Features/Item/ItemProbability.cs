using UnityEngine;

[CreateAssetMenu(fileName = "ItemProbability", menuName = "Scriptable Objects/ItemProbability")]
public class ItemProbability : ScriptableObject
{
    public float common;
    public float rare;
    public float epic;
    public float unique;
    public float legend;
}