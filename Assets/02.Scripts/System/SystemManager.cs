using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시스템 관리자 클래스
/// </summary>
public class SystemManager : Singleton<SystemManager>, IManager
{
    //[추후 변수를 private로 바꾸고 SceneLoader 기능을 SystemManager의 함수로 호출하여 사용하도록 변경 진행 예정]
    // 즉, SceneLoader를 직접 사용하는 것이 아닌 SystemManager를 통해 SceneLoader를 사용하도록 변경할 예정
    public SceneLoader SceneLoader{get; private set;} // 씬 로더            
    public VideoLoader VideoLoader{get; private set;} // 인트로 로더

    /// <summary>
    /// 초기화 함수
    /// </summary>
    public void Initialize()
    {
        if(GameManager.Instance.CurrentScene == SceneConstants.StartScene)      // 시작 씬일 경우
        {
            SceneLoader = GetComponent<SceneLoader>();
            VideoLoader = GetComponent<VideoLoader>();
        }
    }


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
