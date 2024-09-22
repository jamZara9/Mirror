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

    public bool[] qucikSlots = new bool[4];
    public bool isChoiceQuickSlot = false;

    public bool jump = false;
    public bool sprint = false;
    public bool isInventoryVisible = false;
    public bool isQuickSlotVisible = false;
    public bool isInteractable = false;
    public bool isUseItem = false;
    public bool isTransferItem = false;
    public bool isFire = false;
    public bool isSit = false;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public bool analogMovement; // 이동 입력값을 아날로그로 받을지 디지털로 받을지 결정

    private void Start(){
        InputManager.Instance.BindAllActions("Player", this);
    }


    public void OnMovement(InputAction.CallbackContext context)
    {
        // //인벤토리 활성화중 이동 제한
        // if (GameManager.Instance.inventoryManager.Use_Inventory)
        //     return;

        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (cursorInputForLook)
        {
            // 추후 PlayerManager에서 PlayerItemContainer를 이용할 예정 [임시]
            // if (GameManager.Instance.inventoryManager.Use_Inventory) //인벤토리 활성화중 시야이동 제어
            //     look = Vector2.zero;
            // else
                look = context.ReadValue<Vector2>();
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // 추후 PlayerManager에서 PlayerItemContainer를 이용할 예정 [임시]
        // //인벤토리 활성화중 이동 제한
        // if (GameManager.Instance.inventoryManager.Use_Inventory)
        //     return;

        jump = context.ReadValueAsButton();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        // 추후 PlayerManager에서 PlayerItemContainer를 이용할 예정 [임시]
        // //인벤토리 활성화중 기능 제한
        // if (GameManager.Instance.inventoryManager.Use_Inventory)
        //     return;

        sprint = context.ReadValueAsButton();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        isInventoryVisible = context.ReadValueAsButton();
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        // //인벤토리 활성화중 기능 제한
        // if (GameManager.Instance.inventoryManager.Use_Inventory)
        //     return;

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
        // //인벤토리 활성화중 기능 제한
        // if (GameManager.Instance.inventoryManager.Use_Inventory)
        //     return;

        isFire = context.ReadValueAsButton();
    }

    public void OnShowQuickSlot(InputAction.CallbackContext context)
    {
        // //인벤토리 활성화중 기능 제한 [임시 주석처리]
        // if (GameManager.Instance.inventoryManager.Use_Inventory)
        //     return;

        if (context.performed)   // 마우스 휠 버튼을 누르면
        {
            isQuickSlotVisible = true;
        }else if(context.canceled)  // 마우스 휠 버튼을 뗐을 때
        {
            isQuickSlotVisible = false;
        }
    }

    public void OnSit(InputAction.CallbackContext context)
    {
        // 앉기 키를 누르고 있어야 하는 건지? 아니면 한번 누르면 유지되는지 알아야함
        if(context.performed)
        {
            isSit = true;
        }
        else if(context.canceled)
        {
            isSit = false;
        }
    }

    public void OnQuickSlot1(InputAction.CallbackContext context)
    {
        OnQuickSlot(0, context);
    }
    public void OnQuickSlot2(InputAction.CallbackContext context)
    {
        OnQuickSlot(1, context);
    }
    public void OnQuickSlot3(InputAction.CallbackContext context)
    {
        OnQuickSlot(2, context);
    }
    public void OnQuickSlot4(InputAction.CallbackContext context)
    {
        OnQuickSlot(3, context);
    }

    /// <summary>
    /// 퀵슬롯을 선택하는 함수
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="context"></param>
    private void OnQuickSlot(int slotIndex, InputAction.CallbackContext context)
    {
        if (slotIndex < 0 || slotIndex >= qucikSlots.Length)
        {
            Debug.LogError("슬롯 인덱스가 알맞지 않습니다. : " + slotIndex);
            return;
        }

        // 모든 슬롯을 비활성화
        for (int i = 0; i < qucikSlots.Length; i++)
        {
            qucikSlots[i] = false;
        }

        // 선택된 슬롯만 활성화
        qucikSlots[slotIndex] = context.ReadValueAsButton();
        isChoiceQuickSlot = qucikSlots[slotIndex];
    }
}
