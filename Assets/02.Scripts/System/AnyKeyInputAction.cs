using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 모든 키 입력을 수신하는 액션
/// </summary>
public class AnyKeyInputAction : IInputActionStrategy
{
    public bool IsAnyKeyPressed { get; private set; }

    public void BindInputActions(InputActionMap map)
    {
        GameManager.inputManager.BindAllActions(map.name, this);
    }

    public void OnAnyKey(InputAction.CallbackContext context)
    {
        IsAnyKeyPressed = context.ReadValueAsButton();  
    }

}
