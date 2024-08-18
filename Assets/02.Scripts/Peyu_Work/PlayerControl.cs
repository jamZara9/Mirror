using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PlayerControl : MonoBehaviour
{
    // Player 관련
    [SerializeField]
    private float walkSpeed = 3.0f;
    [SerializeField]
    private float sprintSpeed = 6.0f;

    private bool _hasAnimator;
    private int _animIDIsMove;
    private int _animIDIsRun;

    private Transform _playerHeadTr;

    // Camera 관련
    [SerializeField]
    private float lookSensitivity;
    [SerializeField]
    private float cameraRotationLimit;
    private float _currentCameraRotationX;

    // Components
    [SerializeField]
    private GameObject playerCamera;
    private Rigidbody _characterRigid;
    private Animator _animator;

    void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);

        _characterRigid = GetComponent<Rigidbody>();

        AssignAnimationIDs();

        if (_hasAnimator)
        {
            _playerHeadTr = _animator.GetBoneTransform(HumanBodyBones.Head);    // Head 본의 Transform 가져오기
        }
    }

    void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        playerCamera.transform.position = _playerHeadTr.transform.position + _playerHeadTr.transform.up * 0.1f;

        Move();
        CameraRotation();
        ChracterRotation();
    }

    void LateUpdate()
    {
        HeadBoneRotation();
    }

    /// <summary>
    /// 애니메이터 파라미터 해시를 string 형태로 가져오는 함수
    /// </summary>
    private void AssignAnimationIDs()
    {
        _animIDIsMove = Animator.StringToHash("IsMove");
        _animIDIsRun = Animator.StringToHash("IsRun");
    }

    /// <summary>
    /// 플레이어의 기본 이동에 관한 함수
    /// </summary>
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        if ((_moveHorizontal + _moveVertical) != Vector3.zero)
        {
            Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized;

            bool _isRun = Input.GetKey(KeyCode.LeftShift) ? true : false;
            float _targetSpeed = _isRun ? sprintSpeed : walkSpeed;
            _velocity *= _targetSpeed;

            _characterRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDIsMove, true);     // Move 상태로 전환
                _animator.SetBool(_animIDIsRun, _isRun);    // _isRun 값에 따라 Run 파라미터 on/off
            }
        }
        else
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDIsMove, false);    // Idle 상태로 전환
                _animator.SetBool(_animIDIsRun, false);     // Run 파라미터 off
            }
        }
    }

    /// <summary>
    /// 카메라의 위아래 시점 변경에 관한 함수
    /// </summary>
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;

        _currentCameraRotationX -= _cameraRotationX;
        _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        playerCamera.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0f, 0f);
    }

    private void HeadBoneRotation()
    {
        Vector3 HeadDir = playerCamera.transform.position + playerCamera.transform.forward * 10.0f;
        _playerHeadTr.LookAt(HeadDir);
    }

    /// <summary>
    /// 캐릭터의 좌우 회전에 관한 함수
    /// </summary>
    private void ChracterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        _characterRigid.MoveRotation(_characterRigid.rotation * Quaternion.Euler(_characterRotationY)); // 쿼터니언 * 쿼터니언
    }
}
