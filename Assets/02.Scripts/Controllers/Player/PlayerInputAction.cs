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
    public bool isShowInventory = false;
    public bool isPickupItem = false;
    public bool isUseItem = false;
    public bool isTransferItem = false;
    public bool isFire = false;
    public bool isShowQuickSlot = false;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public bool analogMovement; // 이동 입력값을 아날로그로 받을지 디지털로 받을지 결정

    private InputActionAsset _inputActions;

    private void Awake(){
        _inputActions = GetComponent<PlayerInput>().actions;

        BindAllActions("Player");
    }

    /// <summary>
    /// 전달받은 Map의 모든 Action을 받아 그에 맞는 메소드를 바인딩
    /// </summary>
    /// <param name="mapName"></param>
    private void BindAllActions(string mapName)
    {
        // InputActionAsset을 찾아서 할당
        var playerMap = _inputActions.FindActionMap(mapName);

        if (playerMap != null)
        {
            foreach (var action in playerMap.actions)
            {
                // 해당 이름을 가진 메소드를 찾아서 바인딩
                var method = GetType().GetMethod("On" + action.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (method != null)
                {
                    // 해당 메소드를 Delegate로 만들어서 바인딩
                    var del = Delegate.CreateDelegate(typeof(Action<InputAction.CallbackContext>), this, method);
                    action.performed += (Action<InputAction.CallbackContext>)del;
                    action.canceled += (Action<InputAction.CallbackContext>)del;
                }
                else
                {
                    Debug.LogWarning($"Action과 매칭되는 Method를 찾을 수 없습니다. : {action.name}");
                }
            }
        }
        else
        {
            Debug.LogError("해당 이름을 가진 ActionMap을 찾을 수 없습니다.");
        }
    }

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

    public void OnInventory(InputAction.CallbackContext context)
    {
        isShowInventory = context.ReadValueAsButton();
    }

    public void OnPickupItem(InputAction.CallbackContext context)
    {
        isPickupItem = context.ReadValueAsButton();
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        isUseItem = context.ReadValueAsButton();
    }

    public void OnTransferItem(InputAction.CallbackContext context)
    {
        isTransferItem = context.ReadValueAsButton();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        isFire = context.ReadValueAsButton();
    }

    public void OnShowQuickSlot(InputAction.CallbackContext context)
    {
        if(context.performed)   // 마우스 휠 버튼을 누르면
        {
            isShowQuickSlot = true;
        }else if(context.canceled)  // 마우스 휠 버튼을 뗐을 때
        {
            isShowQuickSlot = false;
        }
    }
}
