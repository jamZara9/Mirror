using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utils;

/// <summary>
/// 플레이어의 상태(이동, 점프, 달리기등)에 대한 로직을 담당하는 클래스
/// </summary>
public class PlayerStateController : MonoBehaviour
{
    private PlayerStatus.PlayerBasicSettings _settings;  // PlayerBasicSettings를 참조
    private PlayerStatus _playerStatus;                 // PlayerStatus를 참조
    private float _speed = 0.0f;                    // 현재 속도
    private float _animationBlend;                  // 애니메이션 블렌드
    private float _rotationSmoothTime = 0.12f;      // 회전 부드러움 시간

    [Header("Attack Settings")]
    private float _attackTimeoutDelta = 0.0f;     // 공격 타임아웃 델타

    [Header("Jump Settings")]
    private bool _isGrounded = true;              // 땅에 붙어 있는지 여부
    private float _gravity = -20.0f;              // 중력(기본적으로 -9.81)
    private float _jumpTimeoutDelta = 0.0f;       // 점프 타임아웃 델타
    private float _jumpTimeout = 0.5f;           // 점프 타임아웃
    private float _fallTimeoutDelta;              // 낙하 타임아웃 델타
    private float _fallTimeout = 0.15f;           // 낙하 타임아웃
    private float _verticalVelocity = -2.0f;      // 수직 속도
    private float _terminalVelocity = 53.0f;      // 터미널 속도
    [SerializeField] private LayerMask _groundLayers; // 플레이어가 이동할 수 있는 Ground의 Layer
    private float _groundedOffset = -0.14f;         // 땅과의 거리
    public float _groundedRadius = 0.28f;           // 땅에 붙어 있는지 확인할 반지름(CharacterController의 radius와 동일하게 설정)

    [Header("Animation Settings")]
    private bool _hasFPSAnimator;      // 애니메이터가 있는지 여부
    private bool _has3stAnimator;      // 애니메이터가 있는지 여부
    [SerializeField] private Animator _FPSAnimator;     // 애니메이터 컴포넌트
    [SerializeField] private Animator _3stAnimator;     // 애니메이터 컴포넌트

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private int _animIDAttack;
    private int _animIDSit;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;           // 오디오 소스
    [SerializeField] private AudioClip attackAudioClip;         // 공격 사운드


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
    private bool _isSitVisible = false;                     // 현재 앉기 상태인지 여부

    void Start()
    {
        _hasFPSAnimator = _FPSAnimator != null;
        _has3stAnimator = _3stAnimator != null;

        _playerStatus = PlayerManager.Instance.GetPlayerStatus();
        _settings = _playerStatus.settings;
        _characterController = GetComponent<CharacterController>();
        // _inputActions = GetComponent<PlayerInputAction>();
        // _inputActions = GameManager.Instance.inputManager.playerInputAction;
        _inputActions = InputManager.Instance.playerInputAction;

        AssignAnimationIDs();

        // animator가 있는 경우, 머리 위치를 가져옴
        if (_hasFPSAnimator)
        {
            _playerChestTR = _FPSAnimator.GetBoneTransform(HumanBodyBones.UpperChest);
        }
    }

    void Update()
    {
        OnMovement();
        OnJump();
        CheckGround();

        UseItem();
        InteractionObject();
        TransferItem();
        ShowInventory();
        OnFire();
        ShowQuickSlot();
        OnSit();

        SetSelectItem();

        

        _attackTimeoutDelta += Time.deltaTime;  // 공격 타임아웃 델타 증가
    }

    void LateUpdate()
    {
        CharacterRotation();
        UpdateCameraRotation();
        UpdateChestBoneRotation();

        _cinemachineCamera.transform.position = _playerChestTR.transform.position + _playerChestTR.transform.right * -0.2f;
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
        _animIDAttack = AnimationConstants.AnimIDAttack;
        _animIDSit = AnimationConstants.AnimIDSit;
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

        // 이동 입력에 따라 애니메이션 블렌드를 설정 (앞으로 갈 때 양수, 뒤로 갈 때 음수)
        // _inputActions.move.y가 양수면 전진, 음수면 후진
        float targetBlend = (_inputActions.move.y >= 0) ? targetSpeed : -targetSpeed;

        // 애니메이션 블렌드를 처리하여 이동 애니메이션이 부드럽게 전환되도록 진행
        // _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _settings.speedChangeRate);
        _animationBlend = SmoothSpeedTransition(_animationBlend, targetBlend , _settings.speedChangeRate);

        if (Mathf.Abs(_animationBlend) < 0.01f) _animationBlend = 0f;

        Vector3 moveDirection = new Vector3(_inputActions.move.x, 0, _inputActions.move.y).normalized;

        // 캐릭터가 향하고 있는 방향에 맞게 이동 방향을 변환
        moveDirection = transform.TransformDirection(moveDirection).normalized;

        // 캐릭터를 이동 / 점프
        _characterController.Move(moveDirection * (_speed * Time.deltaTime) + new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);

        // 애니메이터가 존재하는 경우, 애니메이션 상태를 업데이트
        if (_hasFPSAnimator)
        {
            _FPSAnimator.SetFloat(_animIDSpeed, _animationBlend);
            _FPSAnimator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }

        if(_has3stAnimator){
            _3stAnimator.SetFloat(_animIDSpeed, _animationBlend);
            _3stAnimator.SetFloat(_animIDMotionSpeed, inputMagnitude);
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
    /// 카메라 회전 처리
    /// </summary>
    private void UpdateCameraRotation(){
        float _xRotation = _inputActions.look.y * _rotationSmoothTime;    // 상하 회전

        _cinemachineTargetPitch -= _xRotation;
        // _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);
        _cinemachineTargetPitch = MathUtil.ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        Quaternion targetRotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, 0.0f, 0.0f);
        _cinemachineCamera.transform.localRotation = targetRotation;
    }

    /// <summary>
    /// 플레이어  회전 처리
    /// </summary>
    private void UpdateChestBoneRotation(){
        _playerChestTR.localRotation = Quaternion.Euler(0.0f, 0.0f, -_cinemachineCamera.transform.localRotation.eulerAngles.x);
    }

    /// <summary>
    /// 캐릭터 회전 처리
    /// </summary>
    private void CharacterRotation()
    {
        Vector3 _characterRotationY = new Vector3(0f, _inputActions.look.x, 0f) * _rotationSmoothTime;
        _characterController.transform.Rotate(_characterRotationY);
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

            if (_hasFPSAnimator)
            {
                _FPSAnimator.SetBool(_animIDJump, false);
                // _FPSAnimator.SetBool(_animIDFreeFall, false);
            }

            if(_has3stAnimator){
                _3stAnimator.SetBool(_animIDJump, false);
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

                if (_hasFPSAnimator)
                {
                    _FPSAnimator.SetBool(_animIDJump, true);
                }

                if(_has3stAnimator){
                    _3stAnimator.SetBool(_animIDJump, true);
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
            // else
            // {
            //     if (_hasFPSAnimator)
            //     {
            //         _FPSAnimator.SetBool(_animIDFreeFall, true);
            //     }
            // }

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

        if (_hasFPSAnimator)
        {
            _FPSAnimator.SetBool(_animIDGrounded, _isGrounded);
        }

        if(_has3stAnimator){
            _3stAnimator.SetBool(_animIDGrounded, _isGrounded);
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

                    // if(targetItem.UseSound != null) gameManager.itemManager.PlaySound(targetItem.UseSound); // 아이템 사용 사운드 재생
                    audioSource.clip = targetItem.UseSound;
                    audioSource.Play();
                    Debug.Log("아이템 사용");

                }
            }
            _inputActions.isUseItem = false;
        }
    }

    
    private void SetSelectItem(){
        if(_inputActions.isChoiceQuickSlot){
            GameManager gameManager = GameManager.Instance;
            PlayerInventory playerInventory = gameManager.playerInventory;
            InventoryManager inventoryManager = gameManager.inventoryManager;

            // 선택된 퀵슬롯 번호에 해당하는 아이템을 선택
            for(int i = 0; i < _inputActions.qucikSlots.Length; i++){
                if(_inputActions.qucikSlots[i]){
                        // 만일 해당 퀵슬롯에 아이템이 존재하는 경우
                        if(playerInventory.quickSlots[i] != null){
                        // playerInventory.selectedItem = playerInventory.quickSlots[i];
                        playerInventory.selectedItem = playerInventory.quickSlots[i];
                        Debug.Log($"선택된 아이템: {playerInventory.selectedItem.name}");
                    }
                    break;
                }
            }

            

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
                
                //gameManager.playerInventory.AddItem(inventoryItem);// 플레이어 인벤토리에 아이템 추가
                gameManager.inventoryManager.AddItem(inventoryItem);
                // if(inventoryItem.UseSound != null) gameManager.itemManager.PlaySound(inventoryItem.PickSound); // 아이템 획득 사운드 재생
                audioSource.clip = inventoryItem.PickSound;
                audioSource.Play();
                
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

            // 임시 테스트용
            InventoryManager inventoryManager = gameManager.inventoryManager;  // InventoryManager_Test 인스턴스
            UIManager uIManager = gameManager.uiManager;

            // 인벤토리 UI 활성화/비활성화
            inventoryManager.OnShowInventory();
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

    /// <summary>
    /// 공격 처리
    /// </summary>
    private void OnFire()
    {
        if (_inputActions.isFire)
        {
            Debug.Log($"{_attackTimeoutDelta} / {_playerStatus.settings.attackDelay}");

            if(_attackTimeoutDelta > _playerStatus.settings.attackDelay){

                // 애니메이터가 존재하는 경우, 애니메이션 상태를 업데이트
                if (_hasFPSAnimator)
                {
                    // _FPSAnimator.SetBool(_animIDAttack, true);
                    _FPSAnimator.SetTrigger(_animIDAttack);
                }

                if(_has3stAnimator){
                    _3stAnimator.SetTrigger(_animIDAttack);
                }

                Debug.Log("Attack");
                //공격 사거리 내에 적이 있는지 확인
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, _playerStatus.CurrentAttackRange);
                foreach (var hitCollider in hitColliders)
                {
                    // 적인 경우에만 데미지를 입힘
                    if(hitCollider.CompareTag("Enemy")){
                        IDamage target = hitCollider.GetComponent<IDamage>();

                        if(target != null){
                            target.TakeDamage((int)_playerStatus.CurrentAttackDamage);

                            if(audioSource != null){
                                // 공격 사운드 추가
                                audioSource.clip = attackAudioClip;
                                audioSource.volume = 0.5f;
                                audioSource.Play();
                            }

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
    
    /// <summary>
    /// 앉기 처리
    /// </summary>
    private void OnSit()
    {
        if(_inputActions.isSit){
            
            if(!_isSitVisible){
                _isSitVisible = true;

                if (_hasFPSAnimator)
                {
                    _FPSAnimator.SetBool(_animIDSit, true);
                }

                if(_has3stAnimator){
                    _3stAnimator.SetBool(_animIDSit, true);
                }
            }
        }else{
            _isSitVisible = false;

            if (_hasFPSAnimator)
            {
                _FPSAnimator.SetBool(_animIDSit, false);
            }

            if(_has3stAnimator){
                _3stAnimator.SetBool(_animIDSit, false);
            }
        }
    }

    /// <summary>
    /// 일정 시간이 지난 후 공격 애니메이션을 종료하고 원래 상태로 복귀
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator ResetAttackAnimation(float delay)
    {
        // 공격 애니메이션이 끝날 때까지 기다림
        yield return new WaitForSeconds(delay);

        // 애니메이터가 존재하면 공격 애니메이션을 종료
        if (_hasFPSAnimator)
        {
            _FPSAnimator.SetBool(_animIDAttack, false);
        }

        if(_has3stAnimator){
            _3stAnimator.SetBool(_animIDAttack, false);
        }
    }
}


