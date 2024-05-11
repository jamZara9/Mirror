using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpController : MonoBehaviour
{
    private RebindJumping gameActions;

    private Rigidbody rb;

    private void OnEnable()
    {
        gameActions = Input_Manager.inputActions;

        gameActions.GameControls.Jump.started += DoJump;
        gameActions.GameControls.Enable();

        rb = GetComponent<Rigidbody>();
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    }
}
