using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;
using UnityEngine.Events;
using System;

public class DialogueInputAction : MonoBehaviour
{
    [Header("Dialog Settings")]
    public bool isNextSentence = false;
    public bool isSkip = false;
    public bool isCancelSkip = false;
    public bool isRealSkip = false;

    private void Start(){
        // GameManager.Instance.inputManager.BindAllActions("Dialogue", this);
        InputManager.Instance.BindAllActions("Dialogue", this);
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
