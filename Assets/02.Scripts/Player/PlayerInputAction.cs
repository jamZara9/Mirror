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
    [Header("Player Input Values")]
    public Vector2 move;
    public Vector2 look;
    public Vector2 mousePosition;

    public bool qucikSlot1 = false;
    public bool qucikSlot2 = false;
    public bool qucikSlot3 = false;
    public bool qucikSlot4 = false;
    public bool jump = false;
    public bool sprint = false;
    public bool isInventoryVisible = false;
    public bool isQuickSlotVisible = false;
    public bool isInteractable = false;
    public bool isUseItem = false;
    public bool isTransferItem = false;
    public bool isFire = false;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public bool analogMovement; // 이동 입력값을 아날로그로 받을지 디지털로 받을지 결정

    private void Start(){
        GameManager.Instance.inputManager.BindAllActions("Player", this);
    }


    public void OnMovement(InputAction.CallbackContext context)
    {
        //인벤토리 활성화중 이동 제한
        if (GameManager.Instance.inventoryManager.Use_Inventory)
            return;

        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (cursorInputForLook)
        {
            if (GameManager.Instance.inventoryManager.Use_Inventory) //인벤토리 활성화중 시야이동 제어
                look = Vector2.zero;
            else
                look = context.ReadValue<Vector2>();
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //인벤토리 활성화중 이동 제한
        if (GameManager.Instance.inventoryManager.Use_Inventory)
            return;

        jump = context.ReadValueAsButton();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        //인벤토리 활성화중 기능 제한
        if (GameManager.Instance.inventoryManager.Use_Inventory)
            return;

        sprint = context.ReadValueAsButton();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        isInventoryVisible = context.ReadValueAsButton();
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        //인벤토리 활성화중 기능 제한
        if (GameManager.Instance.inventoryManager.Use_Inventory)
            return;

        isInteractable = context.ReadValueAsButton();
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
        //인벤토리 활성화중 기능 제한
        if (GameManager.Instance.inventoryManager.Use_Inventory)
            return;

        isFire = context.ReadValueAsButton();
    }

    public void OnShowQuickSlot(InputAction.CallbackContext context)
    {
        //인벤토리 활성화중 기능 제한
        if (GameManager.Instance.inventoryManager.Use_Inventory)
            return;

        if (context.performed)   // 마우스 휠 버튼을 누르면
        {
            isQuickSlotVisible = true;
        }else if(context.canceled)  // 마우스 휠 버튼을 뗐을 때
        {
            isQuickSlotVisible = false;
        }
    }

    public void OnQuickSlot1(InputAction.CallbackContext context)
    {
        qucikSlot1 = context.ReadValueAsButton();
    }
    public void OnQuickSlot2(InputAction.CallbackContext context)
    {
        qucikSlot2 = context.ReadValueAsButton();
    }
    public void OnQuickSlot3(InputAction.CallbackContext context)
    {
        qucikSlot3 = context.ReadValueAsButton();
    }
    public void OnQuickSlot4(InputAction.CallbackContext context)
    {
        qucikSlot4 = context.ReadValueAsButton();
    }
}
