using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySceneTrigger : MonoBehaviour
{
    private GameController _gameController;
    [SerializeField] private StoryScene myStoryScene; // 내가 실행할 스토리.
    
    private void OnEnable()
    {
        _gameController = FindObjectOfType<GameController>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // 닿으면 내 스토리 실행.
        if (other.CompareTag("Player"))
        {
            _gameController.PlayScene(myStoryScene);

            Destroy(gameObject);
        }
    }
}
