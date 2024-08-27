using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 플레이어의 상태(이동, 점프, 달리기등)에 대한 로직을 담당하는 클래스
/// </summary>
public class PlayerStateController : MonoBehaviour
{
    private PlayerStatus.PlayerBasicSettings _settings;  // PlayerBasicSettings를 참조
    private float _speed = 0.0f;                    // 현재 속도
    private float _animationBlend;                  // 애니메이션 블렌드
    private float _rotationSmoothTime = 0.12f;      // 회전 부드러움 시간

    [Header("Attack Settings")]
    private float _attackTimeoutDelta = 0.0f;     // 공격 타임아웃 델타

    [Header("Jump Settings")]
    private bool _isGrounded = true;              // 땅에 붙어 있는지 여부
    private float _gravity = -14.0f;              // 중력(기본적으로 -9.81)
    private float _jumpTimeoutDelta = 0.0f;       // 점프 타임아웃 델타
    private float _jumpTimeout = 0.5f;           // 점프 타임아웃
    private float _fallTimeoutDelta;              // 낙하 타임아웃 델타
    private float _fallTimeout = 0.15f;           // 낙하 타임아웃
    private float _verticalVelocity;              // 수직 속도
    private float _terminalVelocity = 53.0f;      // 터미널 속도
    [SerializeField] private LayerMask _groundLayers; // 플레이어가 이동할 수 있는 Ground의 Layer
    private float _groundedOffset = -0.14f;         // 땅과의 거리
    public float _groundedRadius = 0.28f;  // 땅에 붙어 있는지 확인할 반지름(CharacterController의 radius와 동일하게 설정)

    [Header("Animation Settings")]
    private bool _hasAnimator;      // 애니메이터가 있는지 여부
    private Animator _animator;     // 애니메이터 컴포넌트

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip landingAudioClip;        // 착지 사운드
    [SerializeField] private AudioClip[] footstepAudioClips;    // 발소리 사운드
    [Range(0, 1)] public float footstepAudioVolume = 0.5f;      // 발소리 사운드 볼륨


    [SerializeField] private PlayerInputAction _inputActions;            // 플레이어 입력 액션
    private CharacterController _characterController;   //  캐릭터 컨트롤러

    // camera settings
    [SerializeField] private GameObject _cinemachineCamera;  // 카메라
    private Transform _playerChestTR;                       // 플레이어의 머리 위치
    private float _cinemachineTargetPitch = 0.0f;           // 현재 카메라 회전 각도
    public float topClamp = 70.0f;                          // 카메라 상단 회전 제한 각도
    public float bottomClamp = -30.0f;                      // 카메라 하단 회전 제한 각도
    public float cameraAngleOverride = 0.0f;                // 카메라 각도 오버라이드

    private bool _isQuickSlotCurrentlyVisible  = false;     // 현재 퀵슬롯 활성화 여부

    void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _settings = GetComponent<PlayerStatus>().settings;
        _characterController = GetComponent<CharacterController>();
        // _inputActions = GetComponent<PlayerInputAction>();
        _inputActions = GameManager.Instance.inputManager.playerInputAction;

        AssignAnimationIDs();

        // animator가 있는 경우, 머리 위치를 가져옴
        if (_hasAnimator)
        {
            _playerChestTR = _animator.GetBoneTransform(HumanBodyBones.Chest);
        }
    }

    void Update()
    {
        OnMovement();
        //CameraRotation();
        CharacterRotation();

        OnJump();
        CheckGround();

        UseItem();
        InteractionObject();
        TransferItem();
        ShowInventory();
        OnFire();
        ShowQuickSlot();

        _cinemachineCamera.transform.position = _animator.GetBoneTransform(HumanBodyBones.UpperChest).transform.position 
            + _animator.GetBoneTransform(HumanBodyBones.UpperChest).transform.right * -0.2f;

        _attackTimeoutDelta += Time.deltaTime;  // 공격 타임아웃 델타 증가
    }

    void LateUpdate()
    {
        // HeadBoneRotation();
        //ChestBoneRotation();
        CameraRotation();
    }


    /// <summary>
    /// 애니메이션 ID를 할당
    /// </summary>
    private void AssignAnimationIDs()
    {
        _animIDSpeed = AnimationConstants.AnimIDSpeed;
        _animIDGrounded = AnimationConstants.AnimIDGrounded;
        _animIDJump = AnimationConstants.AnimIDJump;
        _animIDFreeFall = AnimationConstants.AnimIDFreeFall;
        _animIDMotionSpeed = AnimationConstants.AnimIDMotionSpeed;
    }

    /// <summary>
    /// player 이동 처리
    /// </summary>
    private void OnMovement()
    {
        // 달리기 입력이 있는 경우, 속도를 RunSpeed로 설정
        float targetSpeed = _inputActions.sprint ? _settings.runSpeed : _settings.walkSpeed;

        // 이동 입력이 없는 경우, 속도를 0으로 설정
        if (_inputActions.move == Vector2.zero) targetSpeed = 0.0f;

        // 현재 플레이어의 수평 속도를 계산 (y축 속도는 무시)
        float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

        // 속도 보정값 및 입력 강도를 설정
        float speedOffset = 0.1f;
        float inputMagnitude = _inputActions.analogMovement ? _inputActions.move.magnitude : 1f;

        // 목표 속도와 현재 속도의 차이를 확인하여 가속 또는 감속을 처리
        if (Mathf.Abs(currentHorizontalSpeed - targetSpeed) > speedOffset)
        {
            // // Mathf.Lerp는 선형 보간을 통해 속도를 자연스럽게 변경
            // _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * _settings.speedChangeRate);

            // // 속도를 소수점 3자리까지 반올림하여 처리
            // _speed = Mathf.Round(_speed * 1000f) / 1000f;

            _speed = SmoothSpeedTransition(currentHorizontalSpeed, targetSpeed * inputMagnitude, _settings.speedChangeRate);
        }
        else
        {
            _speed = targetSpeed;
        }

        // 애니메이션 블렌드를 처리하여 이동 애니메이션이 부드럽게 전환되도록 진행
        // _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _settings.speedChangeRate);
        _animationBlend = SmoothSpeedTransition(_animationBlend, targetSpeed, _settings.speedChangeRate);

        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 moveDirection = new Vector3(_inputActions.move.x, 0, _inputActions.move.y).normalized;

        // 캐릭터가 향하고 있는 방향에 맞게 이동 방향을 변환
        moveDirection = transform.TransformDirection(moveDirection).normalized;

        // 캐릭터를 이동 / 점프
        _characterController.Move(moveDirection * (_speed * Time.deltaTime) + new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);

        // 애니메이터가 존재하는 경우, 애니메이션 상태를 업데이트
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    /// <summary>
    /// 속도 보정 로직 처리
    /// </summary>
    /// <param name="currentSpeed"></param>
    /// <param name="targetSpeed"></param>
    /// <param name="speedChangeRate"></param>
    /// <returns></returns>
    private float SmoothSpeedTransition(float currentSpeed, float targetSpeed, float speedChangeRate)
    {
        // Mathf.Lerp를 사용해 현재 속도를 목표 속도로 부드럽게 전환
        float newSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);

        // 속도를 소수점 3자리까지 반올림하여 처리
        return Mathf.Round(newSpeed * 1000f) / 1000f;
    }
    
    /// <summary>
    /// 체스트 본 회전 처리
    /// </summary>
    private void ChestBoneRotation()
    {
        Vector3 chestDir = _cinemachineCamera.transform.position +_cinemachineCamera.transform.forward * 10.0f;
        _playerChestTR.LookAt(chestDir);
    }

    /// <summary>
    /// 카메라 회전 처리
    /// </summary>
    private void CameraRotation()
    {
        float _xRotation = _inputActions.look.y * _rotationSmoothTime;    // 상하 회전

        _cinemachineTargetPitch -= _xRotation;
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        _cinemachineCamera.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, 0.0f, 0.0f);
        _playerChestTR.localRotation = Quaternion.Euler(0.0f, 0.0f, -(_cinemachineTargetPitch + cameraAngleOverride));
    }

    /// <summary>
    /// 캐릭터 회전 처리
    /// </summary>
    private void CharacterRotation()
    {
        float _yRotation = _inputActions.look.x;
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * _rotationSmoothTime;
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

    /// <summary>
    /// 점프 시에 따른 중력, 수직 속도 처리
    /// </summary>
    private void OnJump()
    {
        if (_isGrounded)
        {

            // 지면에 존재할 경우, 낙하 타이머 초기화
            _fallTimeoutDelta = _fallTimeout;

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // 지면에 닿았을 경우, 수직 속도를 초기화
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2.0f;
            }

            // 점프 입력이 있으며, 점프 타이머가 0 이하일 경우
            if (_inputActions.jump && _jumpTimeoutDelta <= 0.0f)
            {

                // 수직 속도를 계산 (원하는 높이까지 올라가기 위한 속도 계산)
                _verticalVelocity = Mathf.Sqrt(_settings.jumpHeight * -2.0f * _gravity);

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            // 점프 타이머 초기화
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // 지면에 닿지 않은 경우, 낙하 타이머 초기화
            _jumpTimeoutDelta = _jumpTimeout;

            // 낙하 타이머 업데이트
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // 지면에 닿았을 때 점프 입력을 비활성화
            _inputActions.jump = false;
        }

        // 점프 시 고점에 도착하고 나서 점차 낙하속도가 증가하도록 처리
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// 지면에 플레이어가 닿았는지 확인을 위한 함수
    /// </summary>
    private void CheckGround()
    {
        //스피어의 위치를 설정, 땅으로부터의 거리를 고려하여 위치를 조정
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);

        //스피어를 이용하여 땅에 닿았는지 여부를 확인
        _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, _isGrounded);
        }
    }

    /// <summary>
    /// 발소리 이벤트
    /// </summary>
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (footstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(_characterController.center), footstepAudioVolume);
            }
        }
    }

    /// <summary>
    /// 착지 이벤트
    /// </summary>
    /// <param name="animationEvent"></param>
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(_characterController.center), footstepAudioVolume);
        }
    }

    /// <summary>
    /// 아이템 사용
    /// @Todo: 추후 로직을 다른 곳으로 이동 시킬지 고민 필요
    /// </summary>
    private void UseItem()
    {
        if (_inputActions.isUseItem)
        {
            GameManager gameManager = GameManager.Instance;
            PlayerInventory playerInventory = gameManager.playerInventory;

            if(playerInventory.selectedItem != null){
                Debug.Log($"선택된 아이템: {playerInventory.selectedItem.name}");
                BaseItem targetItem = playerInventory.selectedItem.GetComponent<BaseItem>();
                if(targetItem.IsUsable == false){
                    Debug.Log("사용할 수 없는 아이템입니다.");
                    _inputActions.isUseItem = false;
                    return;
                }

                if(targetItem.Count > 0){
                    targetItem.UseItem();                        // 아이템 사용
                    targetItem.Count -= 1;                       // 아이템 개수 감소
                    playerInventory.selectedItem = null;
                    Debug.Log("아이템 사용");
                }
            }
            _inputActions.isUseItem = false;
        }
    }

    /// <summary>
    /// 상호작용 오브젝트 처리
    /// @Todo: 아이템 외에도 상호작용 오브젝트에 대한 처리가 필요
    /// </summary>
    private void InteractionObject()
    {
        // 키를 입력 받은 경우 
        if (_inputActions.isInteractable)
        {
            GameManager gameManager = GameManager.Instance;
            CameraController cameraController = gameManager.cameraController;
            GameObject detetedItem = cameraController.detectedObject;
            
            // 아이템이 감지된 경우
            if (detetedItem != null)
            {
                IInventoryItem inventoryItem = detetedItem.GetComponent<IInventoryItem>();
                if(inventoryItem.IsPickable == false){
                    Debug.Log("획득할 수 없는 아이템입니다.");
                    _inputActions.isInteractable = false;
                    return;
                }

                detetedItem.SetActive(false);
                inventoryItem.IsActive = false;
                //inventoryItem.Count += 1; // 아이템 개수 증가
                
                gameManager.playerInventory.AddItem(inventoryItem);// 플레이어 인벤토리에 아이템 추가
                gameManager.inventoryManager.AddItem(inventoryItem);
                Debug.Log("아이템 획득");
                cameraController.detectedObject = null;
            }

            _inputActions.isInteractable = false;
        }
    }

    private void TransferItem()
    {
        if (_inputActions.isTransferItem)
        {

            Debug.Log("아이템 이동");
            // @TODO: 추후 임시코드 제거 및 해당 아이템이 인벤토리에 있는지 확인하는 코드 추가
            // @TODO: 추후 TransferItem 함수를 합칠지 고민 필요

            // 임시 테스트 코드 진행
            // 인벤토리에 있는 아이템 하나를 창고로 이동
            // ItemManager.Instance.TransferItem(PlayerInventory.Instance, Storage.Instance, PlayerInventory.Instance.items[0]);
            _inputActions.isTransferItem = false;
        }
    }

    /// <summary>
    /// 아이템 이동 함수
    /// </summary>
    /// <param name="from">아이템 존재하는 위치</param>
    /// <param name="to">아이템을 이동시킬 위치</param>
    /// <param name="item">전달하고자 하는 아이템</param>
    private void TransferItem(IItemContainer from, IItemContainer to, BaseItem item){
        // @ TODO: 아이템 이동 로직 구현 아직 미완성

        if(from != null && to != null){
            from.RemoveItem(item);
            to.AddItem(item);
            Debug.Log("아이템 이동");
        }else{
            Debug.LogError("아이템 이동 실패");
        }
    }

    private void ShowInventory()
    {
        if (_inputActions.isInventoryVisible)
        {
            GameManager gameManager = GameManager.Instance;     // MainGameManager 인스턴스
            // UIController_Test uiController = gameManager.uiController;  // UIController_Test 인스턴스

            // 임시 테스트용
            InventoryManager inventoryManager = gameManager.inventoryManager;  // InventoryManager_Test 인스턴스
            UIManager uIManager = gameManager.uiManager;

            // 인벤토리 UI 활성화/비활성화
            // uiController.inventoryUI.SetActive(!uiController.inventoryUI.activeSelf);
            inventoryManager.OnShowInventory();
            // gameManager.cameraController.SetCursorState(uiController.inventoryUI.activeSelf);   // 커서 상태 설정
            gameManager.cameraController.SetCursorState(uIManager.Inventory().gameObject.activeSelf);   // 커서 상태 설정

            _inputActions.isInventoryVisible = false;
        }
    }

    private void ShowQuickSlot(){
        // 임시 테스트용
        InventoryManager inventoryManager = GameManager.Instance.inventoryManager; // InventoryManager_Test 인스턴스
        if(_inputActions.isQuickSlotVisible){
            // Debug.Log("퀵슬롯 활성화/비활성화");

            if(!_isQuickSlotCurrentlyVisible){
                _isQuickSlotCurrentlyVisible = true;
                inventoryManager.OnShowQuickSlot();
            }

        }else{
            if(_isQuickSlotCurrentlyVisible){
                _isQuickSlotCurrentlyVisible = false;
                inventoryManager.OnShowQuickSlot();
            }
        }
    }

    private void OnFire()
    {
        
        if (_inputActions.isFire)
        {
            PlayerStatus playerStatus = GetComponent<PlayerStatus>();

            Debug.Log($"{_attackTimeoutDelta} / {playerStatus.settings.attackDelay}");

            if(_attackTimeoutDelta > playerStatus.settings.attackDelay){
                Debug.Log("Attack");
                //공격 사거리 내에 적이 있는지 확인
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, playerStatus.CurrentAttackRange);
                foreach (var hitCollider in hitColliders)
                {
                    // 적인 경우에만 데미지를 입힘
                    if(hitCollider.CompareTag("Enemy")){
                        IDamage target = hitCollider.GetComponent<IDamage>();

                        if(target != null){
                            target.TakeDamage((int)playerStatus.CurrentAttackDamage);
                            Debug.Log($"공격 성공: {hitCollider.name}");                            
                        }
                    }
                }

                _attackTimeoutDelta = 0.0f; // 공격 타임아웃 초기화
            }else{
                Debug.Log("공격 딜레이 중");
            }

            _inputActions.isFire = false;
        }
    }
}


