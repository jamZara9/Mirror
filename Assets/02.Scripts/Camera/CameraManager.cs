using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>, IManager
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;          // 메인 카메라
    [SerializeField] private List<Camera> cameras;      // 서브 카메라 리스트

    // 마우스 커서가 잠겨있는지 확인
    public bool IsCursorLocked{ get { return Cursor.lockState == CursorLockMode.Locked; } }     
    public bool IsCursorVisible{ get{ return Cursor.visible; } } // 마우스 커서가 보이는지 확인

    public void Initialize()
    {
        if(GameManager.Instance.CurrentScene == SceneConstants.StartScene){
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        if(GameManager.Instance.CurrentScene == SceneConstants.PlaygroundA){
            Debug.Log("PlaygroundA 씬에서 카메라 매니저 초기화");
            // 메인 카메라 => 플레이어 카메라 세팅 필요
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            // 현재 씬의 모든 카메라를 찾아 리스트에 추가
            cameras = new List<Camera>(GameObject.FindObjectsOfType<Camera>());
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
