using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : Singleton<SystemManager>, IManager
{
    public VideoLoader VideoLoader{get; private set;} // 인트로 로더

    public void Initialize()
    {
        if(GameManager.Instance.CurrentScene == SceneConstants.StartScene)
        {
            VideoLoader = GetComponent<VideoLoader>();
        }
    }
    
}
