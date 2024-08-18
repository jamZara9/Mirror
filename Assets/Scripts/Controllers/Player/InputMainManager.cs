using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMainManager : Singleton<InputMainManager>
{
    public InputActionAsset inputActions;   // PlayerInputAction이 할당될 변수
    public static InputActionAsset ActionsAsset => Instance.inputActions;

    private void Awake()
    {
        if (!inputActions) throw new NullReferenceException("InputActionAsset이 할당되지 않았습니다.");
        inputActions.Enable();  // InputActionAsset 활성화
    }

    /// <summary>
    /// 해당 이름의 액션을 읽어서 값을 반환
    /// </summary>
    public static bool ReadInput<T>(string actionName, out T value) where T : struct{
        InputAction action = Action(actionName);
        if(action.IsPressed()){
            value = action.ReadValue<T>();
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// 해당 이름의 액션을 반환
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static InputAction Action(string name){
        return ActionsAsset.FindAction(name);
    }

    internal static bool ReadInput(string mOVE, out Vector2 value)
    {
        throw new NotImplementedException();
    }
}
