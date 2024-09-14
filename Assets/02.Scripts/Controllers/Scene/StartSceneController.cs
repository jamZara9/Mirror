using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public void OnStartButtonClicked()
    {
        PlayerPrefs.SetString("NextScene", SceneConstants.PlaygroundB);          // 다음 씬 설정
        _systemManager.SceneLoader.LoadNextScene(SceneConstants.LoadingScene);   // 로딩 씬으로 이동
        _audioManager.StopBackgroundMusic();                                     // 배경음악 정지
    }
}
