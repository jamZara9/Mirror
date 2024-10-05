
using UnityEngine.InputSystem;

public interface IInputActionStrategy 
{
    void BindInputActions(InputActionMap map);  // InputActionMap을 받아서 액션을 바인딩하는 메서드
}
