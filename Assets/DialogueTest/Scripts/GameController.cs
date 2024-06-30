using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UHFPS.Scriptable;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public GameObject dialoguePanel;
    public GameObject movableDialoguePanel;
    
    public StoryScene currentScene;
    
    public DialogueManager dialogueManager;
    public MovableDialogueManager movableDialogueManager;
    
    public BackGroundController backGroundController;

    private bool _isOnSkip = false;
    public GameObject skipPanel;
    public TextMeshProUGUI skipPanelStoryText;

    private DialogueInputAction _dialogueInputAction;
    private InputAction _nextSentenceAction;
    private InputAction _skipAction;
    private InputAction _cancelSkipAction;
    private InputAction _realSkipAction;

    private void Awake()
    {
        dialoguePanel.SetActive(false);
        
        
        _dialogueInputAction = new DialogueInputAction();
        
        _nextSentenceAction = _dialogueInputAction.Dialogue.NextSentence;
        _skipAction = _dialogueInputAction.Dialogue.Skip;
        _cancelSkipAction = _dialogueInputAction.Dialogue.CancelSkip;
        _realSkipAction = _dialogueInputAction.Dialogue.RealSkip;
    }

    private void OnEnable()
    {
        _nextSentenceAction.performed += NextSentence;
        _skipAction.performed += OnSkip;
        _cancelSkipAction.performed += OnSkipCancel;
        _realSkipAction.performed += OnRealSkip;
        
        _nextSentenceAction.Enable();
        _skipAction.Enable();
        _cancelSkipAction.Enable();
        _realSkipAction.Enable();
    }
    
    private void OnDisable()
    {
        _nextSentenceAction.performed -= NextSentence;
        _skipAction.performed -= OnSkip;
        _cancelSkipAction.performed -= OnSkipCancel;
        _realSkipAction.performed -= OnRealSkip;

        _nextSentenceAction.Disable();
        _skipAction.Disable();
        _cancelSkipAction.Disable();
        _realSkipAction.Disable();
    }

    public void PlayScene(StoryScene storyScene, bool isMovable)
    {
        if (!isMovable)
        {
            dialoguePanel.SetActive(true);
            dialogueManager.ParseCSVFile(storyScene);
            dialogueManager.PlayScene();
        }
        else
        {
            movableDialoguePanel.SetActive(true);
            movableDialogueManager.ParseCSVFile(storyScene);
            movableDialogueManager.PlayScene();
        }
    }
    
    bool IsDialogueOn() => dialoguePanel.activeSelf;
    bool IsSkipOn() => _isOnSkip;

    private void NextSentence(InputAction.CallbackContext context)
    {
        if (!IsDialogueOn())
        {
            return;
        }
        
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

    
    private void OnSkip(InputAction.CallbackContext context)
    {
        if (!IsDialogueOn())
        {
            return;
        }
        
        _isOnSkip = true;
        skipPanelStoryText.text = currentScene.summaryText;
        skipPanel.SetActive(true);
    }

    private void OnRealSkip(InputAction.CallbackContext context)
    {
        if (!IsDialogueOn() || !IsSkipOn())
        {
            return;
        }
        
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

    void OnSkipCancel(InputAction.CallbackContext context)
    {
        if (!IsDialogueOn() || !IsSkipOn())
        {
            return;
        }
        
        _isOnSkip = false;
        skipPanel.SetActive(false);
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