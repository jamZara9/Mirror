using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "VideoSetting", menuName = "Settings/VideoSetting")]
public class VideoSetting : ScriptableObject
{
    public VideoClip videoClip;                         // 비디오 클립
    public string skipText = "Please press any key";    // 스킵 텍스트
    public float playbackSpeed = 1f;                    // 재생 속도
    public float volume = 1.0f;                         // 볼륨
    public bool isLoop = false;                         // 반복 재생 여부
    
    // 추가적인 설정 고민해보기
    // ex) 텍스트 위치, 색상, 크기, 폰트 등
}
