using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 플레이어의 상태(이동, 점프, 달리기등)를 관리하는 클래스
/// </summary>
public class PlayerStateController : MonoBehaviour
{

    [Serializable]
    public sealed class BasicSettings
    {
        public float walkSpeed = 2.0f;         // 걷기 속도
        public float runSpeed = 5.335f;        // 달리기 속도
        public float speedChangeRate = 10.0f;  // 속도 변경 비율
        public float jumpHeight = 1;           // 점프 높이
    }

    private BasicSettings settings = new();

    private float _speed = 0.0f;                // 현재 속도
    private float _animationBlend;              // 애니메이션 블렌드
    public float rotationSmoothTime = 0.12f;    // 회전 부드러움 시간


    private bool _hasAnimator;      // 애니메이터가 있는지 여부
    private Animator _animator;     // 애니메이터 컴포넌트

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private PlayerInputAction _inputActions;            // 플레이어 입력 액션
    private CharacterController _characterController;   //  캐릭터 컨트롤러

    // camera settings
    [SerializeField] private GameObject _cinemachineCamera;  // 카메라
    private Transform _playerHeadTr;                        // 플레이어의 머리 위치
    private float _cinemachineTargetPitch = 0.0f;           // 현재 카메라 회전 각도
    public float topClamp = 70.0f;                          // 카메라 상단 회전 제한 각도
    public float bottomClamp = -30.0f;                      // 카메라 하단 회전 제한 각도
    public float cameraAngleOverride = 0.0f;                // 카메라 각도 오버라이드

    void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _characterController = GetComponent<CharacterController>();
        _inputActions = GetComponent<PlayerInputAction>();
        AssignAnimationIDs();

        // animator가 있는 경우, 머리 위치를 가져옴
        if (_hasAnimator)
        {
            _playerHeadTr = _animator.GetBoneTransform(HumanBodyBones.Head);
        }


        // 커서를 화면 중앙에 잠그고 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);
        if (_hasAnimator)
        {
            _animator.SetBool("Grounded", _characterController.isGrounded);
        }


        OnMovement();
        CameraRotation();
        ChracterRotation();

        OnJump();
        OnSprint();
    }

    void LateUpdate()
    {
        HeadBoneRotation();
    }


    /// <summary>
    /// 애니메이션 ID를 할당
    /// </summary>
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    /// <summary>
    /// player 이동 처리
    /// </summary>
    private void OnMovement()
    {
        // 달리기 입력이 있는 경우, 속도를 RunSpeed로 설정
        float speed = _inputActions.sprint ? settings.runSpeed : settings.walkSpeed;

        // 이동 입력이 없는 경우, 속도를 0으로 설정
        if (_inputActions.move == Vector2.zero) speed = 0.0f;

        // 현재 플레이어의 수평 속도를 계산 (y축 속도는 무시)
        float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

        // 속도 보정값 및 입력 강도를 설정
        float speedOffset = 0.1f;
        float inputMagnitude = _inputActions.analogMovement ? _inputActions.move.magnitude : 1f;

        // 목표 속도와 현재 속도의 차이를 확인하여 가속 또는 감속을 처리
        if (Mathf.Abs(currentHorizontalSpeed - speed) > speedOffset)
        {
            // Mathf.Lerp는 선형 보간을 통해 속도를 자연스럽게 변경
            _speed = Mathf.Lerp(currentHorizontalSpeed, speed * inputMagnitude, Time.deltaTime * settings.speedChangeRate);

            // 속도를 소수점 3자리까지 반올림하여 처리
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = speed;
        }

        // 애니메이션 블렌드를 처리하여 이동 애니메이션이 부드럽게 전환되도록 진행
        _animationBlend = Mathf.Lerp(_animationBlend, speed, Time.deltaTime * settings.speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 moveDirection = new Vector3(_inputActions.move.x, 0, _inputActions.move.y).normalized;

        // 캐릭터가 향하고 있는 방향에 맞게 이동 방향을 변환
        moveDirection = transform.TransformDirection(moveDirection).normalized;

        // 캐릭터를 이동
        _characterController.Move(moveDirection * speed * Time.deltaTime);

        // 애니메이터가 존재하는 경우, 애니메이션 상태를 업데이트
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }
    /// <summary>
    /// 머리 회전 처리
    /// </summary>
    private void HeadBoneRotation()
    {
        Vector3 HeadDir = _cinemachineCamera.transform.position + _cinemachineCamera.transform.forward * 10.0f;
        _playerHeadTr.LookAt(HeadDir);
    }

    /// <summary>
    /// 카메라 회전 처리
    /// </summary>
    private void CameraRotation()
    {
        float _xRotation = _inputActions.look.y * rotationSmoothTime;    // 상하 회전

        _cinemachineTargetPitch -= _xRotation;
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        _cinemachineCamera.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, 0.0f, 0.0f);
    }

    /// <summary>
    /// 캐릭터 회전 처리
    /// </summary>
    private void ChracterRotation()
    {
        float _yRotation = _inputActions.look.x;
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * rotationSmoothTime;
        _characterController.transform.Rotate(_characterRotationY);
    }

    /// <summary>
    /// 각도 제한
    /// </summary>
    /// <param name="lfAngle"></param>
    /// <param name="lfMin"></param>
    /// <param name="lfMax"></param>
    /// <returns></returns>
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }


    private void OnJump()
    {

    }

    private void OnSprint()
    {

    }

    /// <summary>
    /// 발소리 이벤트
    /// </summary>
    private void OnFootstep(AnimationEvent animationEvent)
    {

    }

}


