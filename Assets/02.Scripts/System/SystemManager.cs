using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시스템 관리자 클래스
/// </summary>
public class SystemManager : Singleton<SystemManager>, IManager
{       
    public VideoLoader VideoLoader{get; private set;} // 인트로 로더

    private SceneLoader _sceneLoader; // 씬 로더

    /// <summary>
    /// 초기화 함수
    /// </summary>
    public void Initialize(string sceneName)
    {
        if(sceneName == SceneConstants.StartScene)      // 시작 씬일 경우
        {
            if(VideoLoader == null) VideoLoader = GetComponent<VideoLoader>();
        }
    }

    void Start()
    {
        VideoLoader = GetComponent<VideoLoader>();

        _sceneLoader = GetComponent<SceneLoader>();

        Debug.Log($"SceneLoader: {_sceneLoader == null}, VideoLoader: {VideoLoader == null}");
    }

    #region Scene Control
    /// <summary>
    /// 다음 씬을 로드하는 함수
    /// </summary>
    /// <param name="sceneName">씬 이름</param>
    public void LoadNextScene(string sceneName)
    {
        StartCoroutine(_sceneLoader.LoadSceneAsync(sceneName));
    }
    #endregion
    

    #region Mouse Cursor Control
    /// <summary>
    /// 마우스 커서를 숨기고 잠그는 함수
    /// </summary>
    public void LockAndHideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// 마우스 커서를 풀고 보이게 하는 함수
    /// </summary>
    public void UnlockAndShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion
    
}
