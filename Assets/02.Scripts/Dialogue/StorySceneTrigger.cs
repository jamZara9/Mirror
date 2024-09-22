using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySceneTrigger : MonoBehaviour
{
    [SerializeField] private StoryScene myStoryScene; // 내가 실행할 스토리.

    private void OnTriggerEnter(Collider other)
    {
        // 닿으면 내 스토리 실행.
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.StartStoryScene(myStoryScene);

            Destroy(gameObject);
        }
    }
}
