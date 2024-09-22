using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : Singleton<MonsterManager>, IManager
{
    public void Initialize(string sceneName)
    {
        if(sceneName == SceneConstants.PlaygroundA) // 현재 씬이 PlaygroundA라면
        {
            
        }
    }
}
