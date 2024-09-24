using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private SystemManager _systemManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private UIManager _uiManager;

    void Start()
    {
        _systemManager = SystemManager.Instance;
        _audioManager = AudioManager.Instance;
        _uiManager = UIManager.Instance;

        // 게임 시작시 인트로 영상 재생
        _uiManager.SetVideoplayerActive(true);
        _systemManager.VideoLoader.SetupVideoplayer(_uiManager.VideoImage);
        _systemManager.VideoLoader.PlayVedio();
    }

    void Update()
    {
        // @todo: 스킵 텍스트가 다른 컷씬에서도 사용된다면 VideoLoader에 이 기능 추가를 고려
        // 비디오가 재생 중일 때 아무 키나 누르면 영상 정지
        if(_systemManager.VideoLoader.IsVideoPlaying){
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    Debug.Log("Key pressed: " + keyCode);
                    if(_systemManager.VideoLoader.IsActiveSkipText){    // 스킵 텍스트가 활성화되어 있을 때
                        _systemManager.VideoLoader.SkipTextActive(false);    // 스킵 텍스트 비활성화
                        _systemManager.VideoLoader.StopVedio();
                    }else{
                        _systemManager.VideoLoader.SkipTextActive(true);    // 스킵 텍스트 활성화
                    }
                }
            }
        }
        
    }
    
    /// <summary>
    /// 시작 버튼 클릭 시 호출
    /// </summary>
    public void OnStartButtonClicked()
    {
        PlayerPrefs.SetString("NextScene", SceneConstants.PlaygroundA);     // 다음 씬 설정
        _systemManager.LoadNextScene(SceneConstants.LoadingScene);          // 로딩 씬으로 이동
        _audioManager.StopBackgroundMusic();                                // 배경음악 정지
    }
}
