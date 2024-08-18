using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CinemachineVirtualCamera playerCamera;

    PlayerInputHandler input;

    private void Awake()
    {
        TryGetComponent(out input);
    }

}
