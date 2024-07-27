using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySceneTrigger : MonoBehaviour
{
    private GameController gameController;
    [SerializeField] private StoryScene myStoryScene;
    
    private void OnEnable()
    {
        gameController = FindObjectOfType<GameController>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameController.PlayScene(myStoryScene);

            Destroy(gameObject);
        }
    }
}
