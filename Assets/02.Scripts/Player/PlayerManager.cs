using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>, IManager, IDamage
{
    [SerializeField] private PlayerStatus playerStatus; // 플레이어 상태 클래스
	[SerializeField] public Transform playerTransform; // 플레이어 위치

	[SerializeField] private AudioClip[] hitSound;    // 피격 효과음
    [SerializeField] private AudioClip deathSound;    // 사망 효과음

	public void Initialize(string sceneName){
		if(sceneName == SceneConstants.PlaygroundA)
		{
			playerStatus = GetComponent<PlayerStatus>();

			// 추후에는 해당 씬의 Player를 찾아서 할당해야 함
			playerTransform = GameObject.FindGameObjectsWithTag("Player")[0].transform;
		}
	}

	public PlayerStatus GetPlayerStatus(){
		return playerStatus;
	}	

	public void TakeDamage(int hitPower)
    {
        if(playerStatus.CurrentHealth <= 0) return;  // 이미 사망한 경우 데미지를 받지 않음

        playerStatus.AdjustStatus(StatusType.Health, -hitPower);

        // 피격 효과음 재생
        if(playerStatus.CurrentHealth > 0){
            AudioManager.Instance.PlaySoundEffect(hitSound[UnityEngine.Random.Range(0, hitSound.Length)], playerTransform.position, 1.0f);

        }else{
            AudioManager.Instance.PlaySoundEffect(deathSound, playerTransform.position, 1.0f);
        }

        Debug.Log($"현재 체력 : {playerStatus.CurrentHealth}");
    }

}
