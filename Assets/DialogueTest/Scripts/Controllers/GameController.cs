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

    private IStoryEvent _storyEvent;

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

    /// <summary>
    /// 조건에 맞게 다이얼로그 매니저의 스토리 실행 함수를 호출해 스토리를 시작시키는 함수  
    /// </summary>
    /// <param name="storyScene"> 다이얼로그 매니저에 넘겨 실행할 스토리를 인자로 받음 </param>
    /// <param name="storyEvent"> 다이얼로그 매니저에 넘겨 실행할 스토리의 이벤트 인터페이스를 인자로 받음 </param>
    public void PlayScene(StoryScene storyScene, IStoryEvent storyEvent)
    {
        this._storyEvent = storyEvent;
        
        if (!storyScene.isMovableScene)
        {
            dialoguePanel.SetActive(true);
            dialogueManager.ParseCSVFile(storyScene);
            dialogueManager.PlayScene(this._storyEvent);
        }
        else
        {
            movableDialoguePanel.SetActive(true);
            movableDialogueManager.ParseCSVFile(storyScene);
            movableDialogueManager.PlayScene(_storyEvent);
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

    /// <summary>
    /// 스킵 시, 요약본을 띄우고 취소 or 확인의 추가 선택지를 제공하는 함수 
    /// </summary>
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

    /// <summary>
    /// 스킵 버튼 누른 후 요약본 창에서 스킵 확인을 눌렀을 때의 기능을 제공하는 함수
    /// </summary>
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

    /// <summary>
    /// 스킵 시, 추가 선택지를 제공할 때 스킵을 취소하는 함수
    /// </summary>
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
        dialogueManager.EndScene();
        
        skipPanel.SetActive(false);
        _isOnSkip = false;
        
        currentScene = currentScene.nextScene;
        dialogueManager.PlayScene(_storyEvent);
    }

    void EndCurrentStoryScene()
    {
        dialogueManager.EndScene(); 
        dialoguePanel.SetActive(false);
        Time.timeScale = 1;
    }
}