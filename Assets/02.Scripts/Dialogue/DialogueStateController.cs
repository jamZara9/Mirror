using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Dialogue Input에 대한 로직 처리를 담당하는 클래스.
/// </summary>
public class DialogueStateController : MonoBehaviour
{
    [SerializeField] private DialogueInputAction _inputActions;     // 대화 입력 액션

    private GameObject _visualNovelDialoguePanel;                    // 비주얼 노벨 대화 패널
    private GameObject _movableDialoguePanel;                        // 이동 가능한 대화 패널

    private DialogueManager _dialogueManager;                        // 다이얼로그 매니저
    private StoryScene currentScene;                                 // 현재 스토리 씬

    // 스킵 패널 UI 처리에 필요한 변수들.
    private bool _isOnSkip = false;                                  // 스킵 패널이 켜졌는지 체크하는 변수

    /// <summary>
    /// 스킵 UI 패널이 켜졌는지 체크하는 함수.
    /// </summary>
    /// <returns>스킵 UI 패널이 켜져 있는가?</returns>
    private bool IsSkipOn() => _isOnSkip;                     // 스킵 UI 패널이 켜졌는지 체크하는 함수.

    /// <summary>
    /// 다이얼 로그 UI 패널이 켜졌는지 체크하는 함수.
    /// </summary>
    /// <returns>다이얼 로그 UI 패널이 켜져 있는가?</returns>
    private bool IsDialogueOn()
    {
        return _dialogueManager.visualNovelDialoguePanel.activeSelf;
    }

    void Start()
    {
        _inputActions = GetComponent<DialogueInputAction>();
        _dialogueManager = GameManager.Instance.dialogueManager;

        currentScene = _dialogueManager.currentScene;
    }

    void Update()
    {
        NextSentence();
        OnSkip();
        OnRealSkip();
        OnSkipCancel();
    }


    /// <summary>
    /// 다음 문장으로 넘어가는 로직을 처리하는 함수.
    /// </summary>
    /// <param name="context">InputActon</param>
    private void NextSentence()
    {
        if (_inputActions.isNextSentence)
        {
            Debug.Log("NextSentence");

            if (!IsDialogueOn())
            {
                return;
            }

            // 다음 문장으로 넘어갈 수 있는 상태인가?
            if (!_dialogueManager.IsCompleted())
            {
                return;
            }
            // 현재 문장이 마지막 문장인가?
            if (_dialogueManager.IsLastSentence())
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

            _dialogueManager.VisualNovelNextSentence();

        }
    }

    /// <summary>
    /// 스킵 시, 요약본을 띄우고 취소 or 확인의 추가 선택지를 제공하는 함수 
    /// </summary>
    private void OnSkip()
    {
        if (_inputActions.isSkip)
        {
            if (!IsDialogueOn())
            {
                return;
            }
            
            _isOnSkip = true;
            _dialogueManager.skipPanelStoryText.text = currentScene.summaryText;
            _dialogueManager.skipPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 스킵 버튼 누른 후 요약본 창에서 스킵 확인을 눌렀을 때의 기능을 제공하는 함수
    /// </summary>
    private void OnRealSkip()
    {
        if (_inputActions.isRealSkip)
        {
            if (!IsDialogueOn() || !IsSkipOn())
            {
                return;
            }
            
            if (!currentScene.nextScene)
            {
                _dialogueManager.skipPanel.SetActive(false);
                EndCurrentStoryScene();
            }
            else
            {
                NextScene();
            }
        }
    }

    /// <summary>
    /// 스킵 시, 추가 선택지를 제공할 때 스킵을 취소하는 함수
    /// </summary>
    void OnSkipCancel()
    {
        if (_inputActions.isCancelSkip)
        {
            if (!IsDialogueOn() || !IsSkipOn())
            {
                return;
            }
            
            _isOnSkip = false;
            _dialogueManager.skipPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 현재 스토리에서 바로 이어지는 다음 스토리로 넘어가는 함수.
    /// </summary>
    void NextScene()
    {
        _dialogueManager.EndScene();
        
        _dialogueManager.skipPanel.SetActive(false);
        _isOnSkip = false;
        
        currentScene = currentScene.nextScene;
        _dialogueManager.PlayScene(currentScene, currentScene.storyType);
    }

    /// <summary>
    /// 한 스토리가 끝난 후처리 함수.
    /// </summary>
    void EndCurrentStoryScene()
    {
        _dialogueManager.EndScene(); 
        _dialogueManager.visualNovelDialoguePanel.SetActive(false);
        Time.timeScale = 1;
    }
}