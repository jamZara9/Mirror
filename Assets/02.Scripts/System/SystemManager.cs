using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : Singleton<SystemManager>, IManager
{
    public SceneLoader SceneLoader{get; private set;} // 씬 로더
    public VideoLoader VideoLoader{get; private set;} // 인트로 로더

    public void Initialize()
    {
        if(GameManager.Instance.CurrentScene == SceneConstants.StartScene)
        {
            SceneLoader = GetComponent<SceneLoader>();
            VideoLoader = GetComponent<VideoLoader>();
        }
    }
    
}
