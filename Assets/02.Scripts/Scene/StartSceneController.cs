using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI;

public class StartSceneController : MonoBehaviour
{
    private AudioManager _audioManager;
    private UIManager _uiManager;

    [Header("Video Settings")]
    [SerializeField] private List<VideoSetting> _videoSettings; // 비디오 설정 [추후 다른 곳에서 관리를 진행할 수 있음]
    private VideoCanvas videoPlayer;


    void Start()
    {
        _audioManager = GameManager.audioManager;
        _uiManager = GameManager.uiManager;
        
        // 인트로 영상 출력
        videoPlayer = _uiManager.GetOrAddUI<VideoCanvas>();
        videoPlayer.SetVideoSetting(_videoSettings[0]);
        videoPlayer.PlayVideo();
    }

    
    /// <summary>
    /// 시작 버튼 클릭 시 호출 [추후 StartSceneCanvas로 이동]
    /// </summary>
    public void OnStartButtonClicked()
    {
        SceneLoader sceneLoader = new();
        PlayerPrefs.SetString("NextScene", SceneConstants.PlaygroundA);     // 다음 씬 설정

        sceneLoader.LoadNextScene(SceneConstants.LoadingScene);          // 로딩 씬으로 이동
        _audioManager.StopBackgroundMusic();                                // 배경음악 정지
    }
}
