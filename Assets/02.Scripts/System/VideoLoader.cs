using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

/// <summary>
/// 비디오 플레이어를 로드하는 클래스
/// </summary>
public class VideoLoader : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayerPrefab;    // 비디오 플레이어 프리팹
    [SerializeField] private VideoClip[] _videoClip;            // 비디오 클립
    [SerializeField] private TextMeshProUGUI _skipText;         // 스킵 텍스트

    public bool IsActiveSkipText{                       // 스킵 텍스트가 활성화되어 있는지 여부
        get{
            return _skipText.gameObject.activeSelf;
        }
    }

    public bool IsVideoPlaying{get; private set;}   // 비디오가 재생 중인지 여부
    private VideoPlayer _videoPlayer;               // 비디오 플레이어
    private Coroutine _skipTextCoroutine;           // 스킵 텍스트 코루틴

    public void SetupVideoplayer(RawImage targetImage)
    {
        SetupVideoplayer(_videoClip[0], targetImage);
    }

    /// <summary>
    /// 비디오 플레이어 설정
    /// </summary>
    /// <param name="videoClip">재생할 비디오 클립</param>
    /// <param name="targetImage">비디오를 출력할 RawImage</param>
    public void SetupVideoplayer(VideoClip videoClip, RawImage targetImage)
    {
        _videoPlayer = Instantiate(_videoPlayerPrefab, transform);
        _videoPlayer.clip = videoClip;
        
        // 비디오가 끝났을 때 호출될 이벤트 핸들러 등록
        _videoPlayer.loopPointReached += OnVideoFinished;
    }

    /// <summary>
    /// 인트로 영상 시작
    /// </summary>
    public void PlayVedio()
    {
        //@todo: 기존 VideoCanvas를 활성화하는 코드를 추가해야 함
        _videoPlayer.waitForFirstFrame = true;
        IsVideoPlaying = true;
        _videoPlayer.Play();
    }

    public void StopVedio()
    {
        _videoPlayer.Stop();
        OnVideoFinished(_videoPlayer);
    }

    public void PauseVedio()
    {
        _videoPlayer.Pause();
        OnVideoFinished(_videoPlayer);
    }

    /// <summary>
    /// 비디오가 종료되었을 때 호출되는 콜백 함수
    /// </summary>
    /// <param name="vp"></param>
    private  void OnVideoFinished(VideoPlayer vp){
        IsVideoPlaying = false;

        // GameManager에게 비디오가 종료되었음을 알림
        GameManager.Instance.OnVideoFinished();
    }

    /// <summary>
    /// 스킵 텍스트 활성화
    /// </summary>
    public void SkipTextActive(bool isActive)
    {
        if(isActive){
            _skipTextCoroutine = StartCoroutine(ShowSkipText(1.0f, 2.0f));
        }else{

            // 스킵  텍스트 코루틴이 실행 중이면 중지
            if(_skipTextCoroutine != null){
                StopCoroutine(_skipTextCoroutine);
                _skipTextCoroutine = null;
            }
            _skipText.gameObject.SetActive(false);
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
        _skipText.gameObject.SetActive(true);
        _skipText.alpha = 0;
        float elapsed = 0;
        
        while (elapsed < fadeDuration)
        {
            _skipText.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _skipText.alpha = 1;

        // 특정 시간 동안 텍스트를 유지함
        yield return new WaitForSeconds(displayDuration);

        // 텍스트를 천천히 사라지게 함
        elapsed = 0;
        while (elapsed < fadeDuration)
        {
            _skipText.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _skipText.alpha = 0;
        _skipText.gameObject.SetActive(false);
    }

}
