using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSetting", menuName = "Scriptable Objects/LevelSetting")]
public class LevelSetting : ScriptableObject
{
    public List<MobSetting> mobSetList;
    public ItemProbability itemProbability;
    public float nextWaveDelay;
    public float spawnDelay;
}
