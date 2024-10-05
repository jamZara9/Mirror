using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "MonsterScriptable", menuName = "ScriptableObject/MonsterScriptable")]
public class MonsterScriptable : ScriptableObject
{
    public List<GameObject> monsters;
}
