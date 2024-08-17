using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;
using UnityEngine.Events;
using System;

/// <summary>
/// Player의 입력처리를 담당하는 클래스
/// </summary>
public class PlayerInputAction : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool jump = false;
    public bool sprint = false;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public bool analogMovement; // 이동 입력값을 아날로그로 받을지 디지털로 받을지 결정

    public void OnMovement(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (cursorInputForLook)
        {
            look = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jump = context.ReadValueAsButton();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint = context.ReadValueAsButton();
    }
}
