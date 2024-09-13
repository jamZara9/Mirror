using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
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

    /// <summary>
    /// 배경음악 재생
    /// </summary>
    /// <param name="clip">재생할 사운드</param>
    /// <param name="volume">사운드 크기</param>
    public void PlayBackgroundMusic(AudioClip clip, float volume)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
    }
}
