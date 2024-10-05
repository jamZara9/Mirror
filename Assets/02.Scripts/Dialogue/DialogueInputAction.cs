using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;
using UnityEngine.Events;
using System;

// 추후 Dialogue -> Dialog로 변경

public class DialogueInputAction : IInputActionStrategy
{
    public bool isNextSentence = false;
    public bool isSkip = false;
    public bool isCancelSkip = false;
    public bool isRealSkip = false;

    // IInputActionStrategy 인터페이스 메서드
    public void BindInputActions(InputActionMap map)
    {
        GameManager.inputManager.BindAllActions(map.name, this);
    }

    #region Dialogue
    public void OnNextSentence(InputAction.CallbackContext context)
    {
        isNextSentence = context.ReadValueAsButton();
        Debug.Log("NextSentence");
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
