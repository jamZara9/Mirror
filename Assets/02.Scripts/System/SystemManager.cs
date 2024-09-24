using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

/// <summary>
/// 비디오 클립 상수
/// </summary>
public class VideoConstants
{
    public const int INTRO = 0;     // 인트로
}

/// <summary>
/// 시스템 관리자 클래스
/// </summary>
public class SystemManager : Singleton<SystemManager>, IManager
{
    private SceneLoader _sceneLoader; // 씬 로더
    private VideoLoader _videoLoader; // 비디오 로더

    [Header("Video Settings")]
    [SerializeField] private VideoClip[] _videoClip;                // 비디오 클립
    [SerializeField] private TextMeshProUGUI _skipTextGUI;          // 스킵 텍스트
    [SerializeField] private string _skipText;                      // 스킵 텍스트
    private Coroutine _skipTextCoroutine;                           // 스킵 텍스트 코루틴
    public bool IsVideoPlaying{get; private set;}                   // 비디오 재생 여부
    public bool IsSkipTextActive => _skipTextGUI.gameObject.activeSelf;  // 스킵 텍스트 활성화 여부

    /// <summary>
    /// 초기화 함수
    /// </summary>
    public void Initialize(string sceneName)
    {

    }

    void Start()
    {
        _sceneLoader = GetComponent<SceneLoader>();
        _videoLoader = GetComponent<VideoLoader>();

        // 스킵 텍스트 설정
        _skipTextGUI.text = _skipText;

        Debug.Log($"SceneLoader: {_sceneLoader == null}, VideoLoader: {_videoLoader == null}");
    }


    #region Video Control
    // 비디오 재생
    public void PlayVideo(int videoIndex)
    {
        if (_videoLoader != null)
        {
            _videoLoader.SetupVideoplayer(_videoClip[videoIndex]);
            _videoLoader.PlayVedio();
            IsVideoPlaying = true;
        }
    }

    // 비디오 정지
    public void StopVideo()
    {
        if (_videoLoader != null && IsVideoPlaying)
        {
            _videoLoader.StopVedio();
            IsVideoPlaying = false;
        }
    }

    // 비디오 일시 정지
    public void PauseVideo()
    {
        if (_videoLoader != null && IsVideoPlaying)
        {
            _videoLoader.PauseVedio();
            IsVideoPlaying = false;
        }
    }

    /// <summary>
    /// 스킵 텍스트를 활성화/비활성화 하는 함수
    /// </summary>
    /// <param name="isActive">스킵 텍스트 활성화 여부</param>
    public void SetSkipTextActive(bool isActive)
    {
        if (_videoLoader != null)
        {
            if(isActive){
                _skipTextCoroutine = StartCoroutine(ShowSkipText(1.0f, 2.0f));
            }else{

                // 스킵  텍스트 코루틴이 실행 중이면 중지
                if(_skipTextCoroutine != null){
                    StopCoroutine(_skipTextCoroutine);
                    _skipTextCoroutine = null;
                }
                _skipTextGUI.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 스킵 텍스트를 보여주는 코루틴
    /// </summary>
    /// <param name="fadeDuration">텍스트를 온전히 출력하는데 걸리는 시간</param>
    /// <param name="displayDuration">텍스트 유지 시간</param>
    /// <returns></returns>
    private IEnumerator ShowSkipText(float fadeDuration, float displayDuration)
    {
        // 텍스트를 천천히 나타나게 함
        _skipTextGUI.gameObject.SetActive(true);
        _skipTextGUI.alpha = 0;
        float elapsed = 0;
        
        while (elapsed < fadeDuration)
        {
            _skipTextGUI.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _skipTextGUI.alpha = 1;

        // 특정 시간 동안 텍스트를 유지함
        yield return new WaitForSeconds(displayDuration);

        // 텍스트를 천천히 사라지게 함
        elapsed = 0;
        while (elapsed < fadeDuration)
        {
            _skipTextGUI.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _skipTextGUI.alpha = 0;
        _skipTextGUI.gameObject.SetActive(false);
    }
    
    
    #endregion

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
    
    #region Save & Load
    #endregion

    #region keysettings
    #endregion

    #region option settings
    #endregion
}
