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

    void Start()
    {
        _inputActions = GetComponent<DialogueInputAction>();
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
        if(_inputActions.isNextSentence){
           Debug.Log("NextSentence");
        }
    }

    /// <summary>
    /// 스킵 시, 요약본을 띄우고 취소 or 확인의 추가 선택지를 제공하는 함수 
    /// </summary>
    private void OnSkip()
    {
        if(_inputActions.isSkip){
            Debug.Log("OnSkip");
        }
    }

    /// <summary>
    /// 스킵 버튼 누른 후 요약본 창에서 스킵 확인을 눌렀을 때의 기능을 제공하는 함수
    /// </summary>
    private void OnRealSkip()
    {
        if(_inputActions.isRealSkip){
            Debug.Log("OnRealSkip");
        }
    }

   /// <summary>
    /// 스킵 시, 추가 선택지를 제공할 때 스킵을 취소하는 함수
    /// </summary>
    void OnSkipCancel()
    {
        if(_inputActions.isCancelSkip){
            Debug.Log("OnSkipCancel");
        }
    }
}