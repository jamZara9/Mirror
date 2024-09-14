using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// 비디오 플레이어를 로드하는 클래스
/// </summary>
public class VideoLoader : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayerPrefab;    // 비디오 플레이어 프리팹
    [SerializeField] private VideoClip[] _videoClip;            // 비디오 클립

    public bool IsVideoPlaying{get; private set;}   // 비디오가 재생 중인지 여부
    private VideoPlayer _videoPlayer;               // 비디오 플레이어

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

}
