using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputHandler : MonoBehaviour
{
    private GameControls gameActions;
    public Vector2 move;
    public Vector2 look;
    public bool isJump = false;
    public bool isWalk = false;
    public bool isSprint = false;
    public bool isCrouch = false;
    public bool IsFireHeld { get; private set; }
    public bool IsFireDown { get; private set; }
    public bool IsFireUp { get; private set; }
    public bool cursorLocked = true;
    public bool isReloadDown = false;
    public bool IsAiming { get; private set; }


    bool IsSelectWeaponInput { get; set; }
    bool IsSwitchWeaponInput { get; set; }
    int switchToWeaponIndex;
    int selectedNumber;

    Vector2 velo = Vector2.zero;
    private void Awake()
    {
        gameActions = new GameControls();

        IsFireDown = false;
        IsFireHeld = false;
        IsFireUp = false;
        IsAiming = false;
        IsSwitchWeaponInput = false;
        IsSelectWeaponInput = false;

        switchToWeaponIndex = 0;
        selectedNumber = 0;

    }
    void OnEnable()
    {
        gameActions.Player.Enable();
        gameActions.Player.Movement.canceled += DoMovement;
        gameActions.Player.Jump.performed += DoJump;
        gameActions.Player.Sprint.performed += DoSprint;
        gameActions.Player.Walk.performed += DoWalk;
        gameActions.Player.Crouch.performed += DoCrouch;
        gameActions.Player.Reload.performed += DoReload;
        gameActions.Player.Fire.performed += DoFire;
        gameActions.Player.Fire.canceled += DoFire;
        gameActions.Player.SwitchWeapon.performed += DoSwitchWeapon;
        gameActions.Player.SelectWeapon.performed += DoSelectWeapon;

        gameActions.GameMenu.Enable();
        gameActions.GameMenu.Escape.performed += DoEscape;
        gameActions.GameMenu.ScreenClick.performed += DoScreenClick;
    }
    void OnDisable()
    {
        gameActions.Player.Movement.canceled -= DoMovement;
        gameActions.Player.Jump.performed -= DoJump;
        gameActions.Player.Sprint.performed -= DoSprint;
        gameActions.Player.Walk.performed -= DoWalk;
        gameActions.Player.Crouch.performed -= DoCrouch;
        gameActions.Player.Reload.started -= DoReload;
        gameActions.Player.Fire.performed -= DoFire;
        gameActions.Player.Fire.canceled -= DoFire;
        gameActions.Player.SwitchWeapon.performed -= DoSwitchWeapon;
        gameActions.Player.SelectWeapon.performed -= DoSelectWeapon;

        gameActions.GameMenu.Escape.performed -= DoEscape;
        gameActions.GameMenu.ScreenClick.performed -= DoScreenClick;
        gameActions.GameMenu.Disable();
        gameActions.Player.Disable();

    }

    private void Update()
    {
        if (!cursorLocked) return;

        MouseMovement();
        Movement();
        isJump = gameActions.Player.Jump.IsPressed();
        isWalk = gameActions.Player.Walk.IsPressed();
        isCrouch = gameActions.Player.Crouch.IsPressed();
        isSprint = gameActions.Player.Sprint.IsPressed();
        IsAiming = gameActions.Player.Aiming.IsPressed();

        IsFireHeld = gameActions.Player.Fire.IsPressed();
    }
    private void LateUpdate()
    {
        isReloadDown = false;
        IsFireDown = false;
        IsFireUp = false;
        IsSwitchWeaponInput = false;
        IsSelectWeaponInput = false;

    }
    public int GetSwitchWeaponInput()
    {
        if (!cursorLocked) return 0;

        if (!IsSwitchWeaponInput) return 0;

        return switchToWeaponIndex;
    }

    public int GetSelectWeaponInput()
    {
        if (!cursorLocked) return 0;

        if (!IsSelectWeaponInput) return 0;

        return selectedNumber;
    }

    private void DoReload(InputAction.CallbackContext obj)
    {
        if (!cursorLocked) return;


        if (obj.performed)
        {
            isReloadDown = true;
        }
    }

    private void DoSwitchWeapon(InputAction.CallbackContext obj)
    {
        if (!cursorLocked) return;

        if (!obj.performed) return;

        IsSwitchWeaponInput = true;

        switch (obj.control.name)
        {
            case "q":
                switchToWeaponIndex = -1;
                return;
            case "e":
                switchToWeaponIndex = 1;
                return;
        }

    }
    private void DoSelectWeapon(InputAction.CallbackContext obj)
    {
        if (!cursorLocked) return;

        if (!obj.performed) return;

        IsSelectWeaponInput = true;

        selectedNumber = int.Parse(obj.control.name);
    }
    private void DoFire(InputAction.CallbackContext obj)
    {

        if (!cursorLocked) return;

        if (obj.performed) IsFireDown = true;
        else if (obj.canceled) IsFireUp = true;
    }
    private void DoEscape(InputAction.CallbackContext obj)
    {
        if (!obj.performed) return;

        SetCursorState(false);
        look = Vector2.zero;
        move = Vector2.zero;
    }

    private void DoScreenClick(InputAction.CallbackContext obj)
    {
        if (obj.performed) { Debug.Log("click"); SetCursorState(true); }

    }
    private void MouseMovement()
    {
        look = Vector2.ClampMagnitude(gameActions.Player.Look.ReadValue<Vector2>(), 3f);
    }

    private void Movement()
    {
        move = Vector2.SmoothDamp(move, gameActions.Player.Movement.ReadValue<Vector2>(), ref velo, .1f);
    }

    private void DoMovement(InputAction.CallbackContext obj)
    {

    }
    private void DoJump(InputAction.CallbackContext obj)
    {
    }
    private void DoSprint(InputAction.CallbackContext obj)
    {
    }
    private void DoCrouch(InputAction.CallbackContext obj)
    {
    }
    private void DoWalk(InputAction.CallbackContext obj)
    {
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }
    private void SetCursorState(bool newState)
    {
        cursorLocked = newState;
        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !newState;
    }
}
