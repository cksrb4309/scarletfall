using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MobSetting", menuName = "Scriptable Objects/MobSetting")]
public class MobSetting : ScriptableObject
{
    public List<Monster> monsterList;
}
