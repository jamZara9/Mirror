using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventHandler : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;                        // 오디오 소스
    [SerializeField] private AudioClip landingAudioClip;                     // 착지 사운드
    [SerializeField] private AudioClip[] footstepAudioClips;                 // 발소리 사운드
    [SerializeField, Range(0, 1)] private float footstepAudioVolume = 0.5f;  // 발소리 사운드 볼륨
    [SerializeField] private AudioClip attackAudioClip;                      // 공격 사운드
    
    [SerializeField] private CharacterController _characterController;       //  캐릭터 컨트롤러

    #region Animation Events
    /// <summary>
    /// 발소리 이벤트
    /// </summary>
    public void OnFootstep(AnimationEvent animationEvent)
    {
        Debug.Log("Footstep event triggered");
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (footstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, footstepAudioClips.Length);
                // AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(_characterController.center), footstepAudioVolume);
                AudioManager.Instance.PlaySoundEffect(footstepAudioClips[index], transform.TransformPoint(_characterController.center), footstepAudioVolume);
            }
        }
    }

    /// <summary>
    /// 착지 이벤트
    /// </summary>
    /// <param name="animationEvent"></param>
    public void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            // AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(_characterController.center), footstepAudioVolume);
            AudioManager.Instance.PlaySoundEffect(landingAudioClip, transform.TransformPoint(_characterController.center), footstepAudioVolume);
        }
    }
    #endregion
}
