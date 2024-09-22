using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioConstants
{
    public const int BGM_STARTSCENE = 0;
    public const int BGM_PLAYGROUNDA = 1;
}

public class AudioManager : Singleton<AudioManager>, IManager
{
    [SerializeField] private AudioSource bgmSource;     // 배경음악 소스
    [SerializeField] private AudioClip[] bgmClips; // 배경음악 클립들

    public void Initialize(string sceneName)
    {
        if(bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// 한번의 사운드 클립을 재생
    /// </summary>
    /// <param name="clip">재생할 사운드</param>
    /// <param name="position">재생 위치</param>
    /// <param name="volume">사운드 크기</param>
    public void PlaySoundEffect(AudioClip clip, Vector3 position, float volume)
    {
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void PlayBackgroundMusic(int index)
    {
        try{
            PlayBackgroundMusic(bgmClips[index], 1.0f);
        }catch(System.IndexOutOfRangeException e){
            Debug.LogError("인덱스 범위를 넘어감: " + e.Message);
        }
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
