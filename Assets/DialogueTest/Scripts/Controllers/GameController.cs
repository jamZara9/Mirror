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
    // 각 스토리 진행 방식에 따른 UI 패널.
    public GameObject dialoguePanel;
    public GameObject movableDialoguePanel;
    
    // 각 스토리 진행 방식에 따른 매니저 스크립트. 
    public DialogueManager dialogueManager;
    public MovableDialogueManager movableDialogueManager;
    
    public StoryScene currentScene;

    // 스킵 패널 UI 처리에 필요한 변수들.
    private bool _isOnSkip = false;
    public GameObject skipPanel;
    public TextMeshProUGUI skipPanelStoryText;

    // New Input System에 따른 입력 처리 Acton.
    private DialogueInputAction _dialogueInputAction;
    private InputAction _nextSentenceAction;
    private InputAction _skipAction;
    private InputAction _cancelSkipAction;
    private InputAction _realSkipAction;

    private void Awake()
    {
        dialoguePanel.SetActive(false);

        // Input Acton 세팅.
        _dialogueInputAction = new DialogueInputAction();
        
        _nextSentenceAction = _dialogueInputAction.Dialogue.NextSentence;
        _skipAction = _dialogueInputAction.Dialogue.Skip;
        _cancelSkipAction = _dialogueInputAction.Dialogue.CancelSkip;
        _realSkipAction = _dialogueInputAction.Dialogue.RealSkip;
    }

    private void OnEnable()
    {
        // Input Acton 세팅(시작).
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
        // Input Acton 세팅(끝).
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
    /// <param name="storyScene">다이얼로그 매니저에 넘겨 실행할 스토리</param>
    public void PlayScene(StoryScene storyScene)
    {
        if (!storyScene.isMovableScene)
        {
            // VN 방식 스토리 진행.
            dialoguePanel.SetActive(true);
            dialogueManager.ParseCSVFile(storyScene);
            dialogueManager.PlayScene();
        }
        else
        {
            // 움직일 수 있는 방식 스토리 진행.
            movableDialoguePanel.SetActive(true);
            movableDialogueManager.ParseCSVFile(storyScene);
            movableDialogueManager.PlayScene();
        }
    }
    
    /// <summary>
    /// 다이얼 로그 UI 패널이 켜졌는지 체크하는 함수.
    /// </summary>
    /// <returns>다이얼 로그 UI 패널이 켜져 있는가?</returns>
    bool IsDialogueOn() => dialoguePanel.activeSelf;
    
    /// <summary>
    /// 스킵 UI 패널이 켜졌는지 체크하는 함수.
    /// </summary>
    /// <returns>스킵 UI 패널이 켜져 있는가?</returns>
    bool IsSkipOn() => _isOnSkip;

    /// <summary>
    /// 다음 문장으로 넘어가는 로직을 처리하는 함수.
    /// </summary>
    /// <param name="context">InputActon</param>
    private void NextSentence(InputAction.CallbackContext context)
    {
        if (!IsDialogueOn())
        {
            return;
        }
        
        // 다음 문장으로 넘어갈 수 있는 상태인가?
        if (!dialogueManager.IsCompleted())
        {
            return;
        }
        // 현재 문장이 마지막 문장인가?
        if (dialogueManager.IsLastSentence())
        {
            // 현재 스토리에 바로 이어지는 다음 스토리가 있는가? 
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

    /// <summary>
    /// 현재 스토리에서 바로 이어지는 다음 스토리로 넘어가는 함수.
    /// </summary>
    void NextScene()
    {
        dialogueManager.EndScene();
        
        skipPanel.SetActive(false);
        _isOnSkip = false;
        
        currentScene = currentScene.nextScene;
        dialogueManager.PlayScene();
    }

    /// <summary>
    /// 한 스토리가 끝난 후처리 함수.
    /// </summary>
    void EndCurrentStoryScene()
    {
        dialogueManager.EndScene(); 
        dialoguePanel.SetActive(false);
        Time.timeScale = 1;
    }
}