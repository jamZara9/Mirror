using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 모든 키 입력을 수신하는 액션
/// </summary>
public class AnyKeyInputAction : IInputActionStrategy
{
    private InputActionMap _anyKeyActionMap;
    private InputAction _anyKeyAction;

    public bool IsAnyKeyPressed { get; private set; }

    public AnyKeyInputAction()
    {
        // 새로운 AnyKey ActionMap 생성
        _anyKeyActionMap = new InputActionMap("AnyKey");

        // AnyKey 액션 추가 - 모든 키 입력을 받음
        _anyKeyAction = _anyKeyActionMap.AddAction("AnyKey", InputActionType.Button, "<Keyboard>/anyKey");
    }

    public void BindInputActions(InputActionMap map)
    {
        // AnyKey 입력 바인딩
        _anyKeyAction.performed += OnAnyKeyPressed;
        _anyKeyAction.Enable();
    }

    private void OnAnyKeyPressed(InputAction.CallbackContext context)
    {
        // AnyKey 입력이 발생했을 때 처리
        Debug.Log("Any key was pressed!");
        IsAnyKeyPressed = context.ReadValueAsButton();
    }

    public void UnbindInputActions()
    {
        _anyKeyAction.Disable();
        _anyKeyAction.performed -= OnAnyKeyPressed;
    }

    // ActionMap 반환 (InputManager에서 관리할 수 있도록 제공)
    public InputActionMap GetActionMap()
    {
        return _anyKeyActionMap;
    }
}
