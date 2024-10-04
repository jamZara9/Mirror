using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class AudioManager : IManager
{
    private AudioSource bgmSource;         // 배경음악 소스
    public void Initialize(string sceneName)
    {
       
    }

    /// <summary>
    /// 배경음악 클립들을 설정
    /// </summary>
    /// <param name="source">배경음이 출력될 AudioSource</param>
    public void SetBGMSource(AudioSource source)
    {
        if(source == null)
        {
            Debug.LogError("AudioSource가 설정되지 않았습니다.");
            return;
        }
        
        bgmSource = source;
    }

    /// <summary>
    /// 한번의 사운드 클립을 재생
    /// </summary>
    /// <param name="clip">재생할 사운드</param>
    /// <param name="position">재생 위치</param>
    /// <param name="volume">사운드 크기</param>
    public void PlaySoundEffect(AudioClip clip, Vector3 position, float volume)
    {
        if(clip == null)
        {
            Debug.LogError("재생할 AudioClip이 없습니다.");
            return;
        }

        if(position == null)
        {
            Debug.LogError("재생 위치가 없습니다.");
            return;
        }

        if(volume < 0.0f || volume > 1.0f)
        {
            Debug.LogError("볼륨이 올바르지 않습니다.");
            return;
        }
        

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        PlayBackgroundMusic(clip, 1.0f);
    }

    /// <summary>
    /// 배경음악 재생
    /// </summary>
    /// <param name="clip">재생할 사운드</param>
    /// <param name="volume">사운드 크기</param>
    public void PlayBackgroundMusic(AudioClip clip, float volume)
    {
        if(bgmSource == null)
        {
            Debug.LogError("BGM AudioSource가 설정되지 않았습니다.");
            return;
        }

        if(clip == null)
        {
            Debug.LogError("재생할 AudioClip이 없습니다.");
            return;
        }
        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBackgroundMusic()
    {
        bgmSource.Stop();
    }
}
