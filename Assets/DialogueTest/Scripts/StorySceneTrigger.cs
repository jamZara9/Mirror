using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySceneTrigger : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private StoryScene myStoryScene;
    [SerializeField] private bool isMovableStory;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameController.PlayScene(myStoryScene, isMovableStory);
            this.gameObject.SetActive(false);
        }
    }
}
