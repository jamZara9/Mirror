using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject dialoguePanel;
    public StoryScene currentScene;
    public DialogueManager dialogueManager;
    public BackGroundController backGroundController;

    private bool _isOnSkip = false;
    public GameObject skipPanel;
    public TextMeshProUGUI skipPanelStoryText;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void PlayScene(StoryScene storyScene)
    {
        dialoguePanel.SetActive(true);
        dialogueManager.ParseCSVFile(storyScene);
        dialogueManager.PlayScene();
    }

    public void OnSkip()
    {
        _isOnSkip = true;
        skipPanelStoryText.text = currentScene.summaryText;
        skipPanel.SetActive(true);
    }

    public void OnRealSkip()
    {
        if (!currentScene.nextScene)
        {
            skipPanel.SetActive(false);
            EndCurrentStoryScene();
        }
        else
        {
            NextScene();
        }
    }

    void OnSkipCancel()
    {
        _isOnSkip = false;
        skipPanel.SetActive(false);
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
                    EndCurrentStoryScene();
                }
                else
                {
                    NextScene();
                }

                return;
            }
                
            dialogueManager.PlayNextSentence();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            OnSkip();
        }
        if (_isOnSkip)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                OnRealSkip();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                OnSkipCancel();
            }
        }
    }

    void NextScene()
    {
        currentScene = currentScene.nextScene;
        dialogueManager.PlayScene();
    }

    void EndCurrentStoryScene()
    {
        dialogueManager.EndScene(); 
        dialoguePanel.SetActive(false);
        Time.timeScale = 1;
    }
}