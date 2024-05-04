using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject dialoguePanel;
    public StoryScene currentScene;
    public DialogueManager dialogueManager;
    public BackGroundController backGroundController;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void PlayScene(StoryScene storyScene)
    {
        // Time.timeScale = 0;
        dialoguePanel.SetActive(true);
        dialogueManager.ParseCSVFile(storyScene);
        dialogueManager.PlayScene();
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!dialogueManager.IsCompleted())
            {
                return;
            }
            
            if (dialogueManager.IsLastSentence())
            {
                if (!currentScene.nextScene)
                {
                    dialoguePanel.SetActive(false);
                    Time.timeScale = 1;
                    
                }
                else
                {
                    currentScene = currentScene.nextScene;
                    dialogueManager.PlayScene();
                }

                return;
            }
                
            dialogueManager.PlayNextSentence();
        }
    }
}