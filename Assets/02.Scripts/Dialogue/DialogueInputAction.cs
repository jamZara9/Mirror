using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;
using UnityEngine.Events;
using System;


public class DialogueInputAction : IInputActionStrategy
{
    [Header("Dialog Settings")]
    public bool isNextSentence = false;
    public bool isSkip = false;
    public bool isCancelSkip = false;
    public bool isRealSkip = false;

    // IInputActionStrategy 인터페이스 메서드
    public void BindInputActions(InputActionMap map)
    {
        // Dialogue 관련 액션들을 바인딩
        map["NextSentence"].performed += OnNextSentence;
        map["Skip"].performed += OnSkip;
        map["CancelSkip"].performed += OnCancelSkip;
        map["RealSkip"].performed += OnRealSkip;
    }

    #region Dialogue
    public void OnNextSentence(InputAction.CallbackContext context)
    {
        isNextSentence = context.ReadValueAsButton();
    }
    public void OnSkip(InputAction.CallbackContext context)
    {
        isSkip = context.ReadValueAsButton();
    }
    public void OnCancelSkip(InputAction.CallbackContext context)
    {
        isCancelSkip = context.ReadValueAsButton();
    }
    public void OnRealSkip(InputAction.CallbackContext context)
    {
        isRealSkip = context.ReadValueAsButton();
    }
    #endregion
}
