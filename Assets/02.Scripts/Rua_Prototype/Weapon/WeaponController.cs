using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInputHandler))]
public class WeaponController : MonoBehaviour
{
    public enum WeaponSwitchState
    {
        Up,
        Down,
        PutDownPrevious,
        PutUpNew,
    }

    [Header("The Player will start with")]
    public List<Weapon> startingWeapons = new List<Weapon>();

    [Header("References")]
    public Camera weaponCamera;
    public Transform weaponParentSocket;
    public Transform defaultWeaponPosition;
    public Transform AimingWeaponPosition;
    public Transform downWeaponPosition;

    [Header("Weapon Bob")]
    public float bobFrequency = 10f;
    public float bobSharpness = 10f;
    public float defaultBobAmount = 0.05f;
    public float aimingBobAmount = 0.02f;

    [Header("Weapon Recoil : (it's only set gun weapon)")]
    public float recoilSharpness = 50f;
    public float maxRecoilDistance = 0.5f;
    public float recoilRestitutionSharpness = 10f;

    [Header("Misc")]
    public float aimingAnimationSpeed = 10f;
    public float defaultFov = 60f;
    public float weaponFovMultiplier = 1f;
    public float weaponSwitchDelay = 1f;
    public LayerMask fpsWeaponLayer;

    public bool IsAiming { get; private set; }
    public bool IsPointingAtEnemy { get; private set; }
    public int ActiveWeaponIndex { get; private set; }

    public UnityAction<Weapon> OnSwitchedToWeapon;
    public UnityAction<Weapon, int> OnAddedWeapon;
    public UnityAction<Weapon, int> OnRemovedWeapon;
    Weapon[] weaponSlots = new Weapon[3];
    PlayerInputHandler input;
    PlayerController playerController;
    PlayerMovement playerMovement;
    float weaponBobFactor;
    Vector3 lastCharacterPosition;
    Vector3 weaponMainLocalPosition;
    Vector3 weaponBobLocalPosition;
    Vector3 weaponRecoilLocalPosition;
    Vector3 accumlateRecoil;
    float timeStartedWeaponSwitch;
    WeaponSwitchState weaponSwitchState;
    int weaponSwitchNewWeaponIndex;

    private void Start()
    {
        ActiveWeaponIndex = -1;
        weaponSwitchState = WeaponSwitchState.Down;

        TryGetComponent(out input);
        TryGetComponent(out playerController);
        TryGetComponent(out playerMovement);

        SetFov(defaultFov);

        OnSwitchedToWeapon += OnWeaponSwitched;

        foreach (var weapon in startingWeapons)
        {
            AddWeapon(weapon);
        }

        SwitchWeapon(true);
    }

    private void Update()
    {
        Weapon activeWeapon = GetActiveWeapon();

        if (activeWeapon != null)
        {
            switch (activeWeapon.weaponType)
            {
                case WeaponType.Gun:
                    if (!GunUpdate(activeWeapon as Gun)) return;
                    break;
                case WeaponType.Melee:
                    break;
                default:
                    return;
            }
        }

        if (!IsAiming && (weaponSwitchState == WeaponSwitchState.Up || weaponSwitchState == WeaponSwitchState.Down))
        {
            int switchWeaponInput = input.GetSwitchWeaponInput();

            if (switchWeaponInput != 0)
            {
                SwitchWeapon(switchWeaponInput > 0);
            }
            else
            {
                switchWeaponInput = input.GetSelectWeaponInput();
                if (switchWeaponInput != 0)
                {
                    if (GetWeaponAtSlotIndex(switchWeaponInput - 1) != null)
                    {
                        SwitchToWeaponIndex(switchWeaponInput - 1);
                    }
                }
            }
        }
        IsPointingAtEnemy = false;

        // 에임에 적이 있을경우
        if (activeWeapon)
        {
            if (Physics.Raycast(weaponCamera.transform.position, weaponCamera.transform.forward, out RaycastHit hit,
            1000, -1, QueryTriggerInteraction.Ignore))
            {
                // if (hit.collider.GetComponentInParent<Health>() != null)
                // {
                //     IsPointingAtEnemy = true;
                // }
            }
        }
    }

    void LateUpdate()
    {
        Weapon activeWeapon = GetActiveWeapon();
        if (activeWeapon == null) return;

        switch (activeWeapon.weaponType)
        {
            case WeaponType.Gun:
                UpdateWeaponAiming(activeWeapon as Gun);
                UpdateWeaponRecoil();
                break;
            case WeaponType.Melee:
                break;
        }

        UpdateWeaponBob();
        UpdateWeaponSwitching();

        weaponParentSocket.localPosition = weaponMainLocalPosition + weaponBobLocalPosition + weaponRecoilLocalPosition;
    }

    private bool GunUpdate(Gun activeWeapon)
    {
        if (activeWeapon.IsReloading) return false;

        if (weaponSwitchState == WeaponSwitchState.Up)
        {
            if (!activeWeapon.automaticReload && input.isReloadDown && activeWeapon.CurrentAmmoRatio < 1f)
            {
                IsAiming = false;
                activeWeapon.StartReloadAnimation();
                return false;
            }

            IsAiming = input.IsAiming;

            bool hasFired = activeWeapon.HandleAttackInputs(
                input.IsFireDown,
                input.IsFireHeld,
                input.IsFireUp
            );

            if (hasFired)
            {
                accumlateRecoil += Vector3.back * activeWeapon.recoilForce;
                accumlateRecoil = Vector3.ClampMagnitude(accumlateRecoil, maxRecoilDistance);
            }

        }

        return true;
    }

    // 중복 소지 방지
    public Weapon HasWeapon(Weapon weaponPrefab)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            var w = weaponSlots[i];
            if (w != null && w.SourcePrefab == weaponPrefab.gameObject) return w;
        }

        return null;
    }

    void UpdateWeaponAiming(Gun activeWeapon)
    {
        if (weaponSwitchState == WeaponSwitchState.Up)
        {
            if (activeWeapon.weaponType != WeaponType.Gun) return;

            if (IsAiming && activeWeapon)
            {
                weaponMainLocalPosition = Vector3.Lerp(
                    weaponMainLocalPosition,
                    AimingWeaponPosition.localPosition + activeWeapon.aimOffset,
                    aimingAnimationSpeed * Time.deltaTime
                );

                SetFov(Mathf.Lerp(
                    playerController.playerCamera.m_Lens.FieldOfView,
                    activeWeapon.aimZoomRatio * defaultFov,
                    aimingAnimationSpeed * Time.deltaTime
                ));
            }
            else
            {
                weaponMainLocalPosition = Vector3.Lerp(
                    weaponMainLocalPosition,
                    defaultWeaponPosition.localPosition,
                    aimingAnimationSpeed * Time.deltaTime
                );

                SetFov(Mathf.Lerp(
                    playerController.playerCamera.m_Lens.FieldOfView,
                    defaultFov,
                    aimingAnimationSpeed * Time.deltaTime
                ));
            }
        }
    }
    void UpdateWeaponBob()
    {
        if (Time.deltaTime <= 0f) return;

        Vector3 playerCharacterVelocity = (playerController.transform.position - lastCharacterPosition) / Time.deltaTime;

        float characterMovementFactor = 0f;
        if (playerMovement.isGround)
        {
            characterMovementFactor = Mathf.Clamp01(
                playerCharacterVelocity.magnitude / playerMovement.sprintSpeed);
        }

        weaponBobFactor = Mathf.Lerp(weaponBobFactor, characterMovementFactor, bobSharpness * Time.deltaTime);

        float bobAmount = IsAiming ? aimingBobAmount : defaultBobAmount;
        float frequency = bobFrequency;
        float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * weaponBobFactor;
        float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount * weaponBobFactor;

        weaponBobLocalPosition.x = hBobValue;
        weaponBobLocalPosition.y = Mathf.Abs(vBobValue);
        lastCharacterPosition = playerController.transform.position;
    }

    void UpdateWeaponRecoil()
    {
        if (weaponRecoilLocalPosition.z >= accumlateRecoil.z * 0.99f)
        {
            weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, accumlateRecoil, recoilSharpness * Time.deltaTime);

        }
        else
        {
            weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, Vector3.zero, recoilRestitutionSharpness * Time.deltaTime);
            accumlateRecoil = weaponRecoilLocalPosition;
        }
    }

    void UpdateWeaponSwitching()
    {
        float switchingTimeFactor = 0f;
        if (weaponSwitchDelay == 0f)
        {
            switchingTimeFactor = 1f;
        }
        else
        {
            switchingTimeFactor = Mathf.Clamp01((Time.time - timeStartedWeaponSwitch) / weaponSwitchDelay);
        }

        if (switchingTimeFactor >= 1f)
        {
            // if (weaponSwitchState == WeaponSwitchState.PutDownPrevious)
            // {


            // }
            // else 
            if (weaponSwitchState == WeaponSwitchState.PutUpNew)
            {
                weaponSwitchState = WeaponSwitchState.Up;
            }
        }

        // if (weaponSwitchState == WeaponSwitchState.PutDownPrevious)
        // {
        //     weaponMainLocalPosition = Vector3.Lerp(defaultWeaponPosition.localPosition, downWeaponPosition.localPosition, switchingTimeFactor);
        // }
        // else 
        // 애니메이션
        if (weaponSwitchState == WeaponSwitchState.PutUpNew)
        {
            weaponMainLocalPosition = Vector3.Lerp(downWeaponPosition.localPosition, defaultWeaponPosition.localPosition, switchingTimeFactor);
        }
    }

    public bool AddWeapon(Weapon weaponPrefab)
    {
        if (HasWeapon(weaponPrefab) != null) return false;

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] != null) continue;

            Weapon weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
            weaponInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            weaponInstance.Owner = gameObject;
            weaponInstance.SourcePrefab = weaponPrefab.gameObject;
            weaponInstance.ShowWeapon(false);

            int layerIndex = Mathf.RoundToInt(Mathf.Log(fpsWeaponLayer.value, 2));
            foreach (Transform t in weaponInstance.gameObject.GetComponentInChildren<Transform>(true))
            {
                t.gameObject.layer = layerIndex;
            }
            weaponSlots[i] = weaponInstance;

            OnAddedWeapon?.Invoke(weaponInstance, i);
            return true;
        }

        // 손에 든 무기가 없을때
        if (GetActiveWeapon() == null)
        {
            SwitchWeapon(true);
        }

        return false;

    }

    public bool RemoveWeapon(Weapon weaponInstance)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] != weaponInstance) continue;

            weaponSlots[i] = null;

            OnRemovedWeapon?.Invoke(weaponInstance, i);

            Destroy(weaponInstance.gameObject);

            if (i == ActiveWeaponIndex) SwitchWeapon(true);

            return true;
        }

        return false;
    }
    public void SetFov(float fov)
    {
        playerController.playerCamera.m_Lens.FieldOfView = fov;
        weaponCamera.fieldOfView = fov * weaponFovMultiplier;
    }
    public void SwitchWeapon(bool ascendingOrder)
    {
        int newWeaponIndex = -1;
        int closestSlotDistance = weaponSlots.Length;
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (i != ActiveWeaponIndex && GetWeaponAtSlotIndex(i) != null)
            {
                int distanceToActiveIndex = GetDistanceBetweenWeaponSlots(ActiveWeaponIndex, i, ascendingOrder);

                if (distanceToActiveIndex < closestSlotDistance)
                {
                    closestSlotDistance = distanceToActiveIndex;
                    newWeaponIndex = i;
                }
            }
        }

        SwitchToWeaponIndex(newWeaponIndex);
    }
    public void SwitchToWeaponIndex(int newWeaponIndex, bool force = false)
    {
        if (force || (newWeaponIndex != ActiveWeaponIndex && newWeaponIndex >= 0))
        {
            weaponSwitchNewWeaponIndex = newWeaponIndex;
            timeStartedWeaponSwitch = Time.time;

            if (GetActiveWeapon() == null)
            {
                weaponMainLocalPosition = downWeaponPosition.localPosition;
                weaponSwitchState = WeaponSwitchState.PutUpNew;
                ActiveWeaponIndex = weaponSwitchNewWeaponIndex;

                Weapon newWeapon = GetWeaponAtSlotIndex(weaponSwitchNewWeaponIndex);

                OnSwitchedToWeapon?.Invoke(newWeapon);
            }
            else
            {
                SwitchWeaponState();
            }
        }
    }

    void SwitchWeaponState()
    {
        weaponSwitchState = WeaponSwitchState.PutDownPrevious;

        // 현재 쓰고 있는건 숨김처리
        Weapon oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
        if (oldWeapon != null)
        {
            oldWeapon.ShowWeapon(false);
        }

        ActiveWeaponIndex = weaponSwitchNewWeaponIndex;
        //switchingTimeFactor = 0f;

        Weapon newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
        OnSwitchedToWeapon?.Invoke(newWeapon);

        if (newWeapon)
        {
            timeStartedWeaponSwitch = Time.time;

            weaponSwitchState = WeaponSwitchState.PutUpNew;
        }
        else
        {
            weaponSwitchState = WeaponSwitchState.Down;
        }
    }
    public Weapon GetActiveWeapon()
    {
        return GetWeaponAtSlotIndex(ActiveWeaponIndex);
    }

    public Weapon GetWeaponAtSlotIndex(int index)
    {
        if (index >= 0 && index < weaponSlots.Length) return weaponSlots[index];

        return null;
    }
    int GetDistanceBetweenWeaponSlots(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
    {
        int distanceBetweenSlots;

        // 오름차순
        if (ascendingOrder)
        {
            distanceBetweenSlots = toSlotIndex - fromSlotIndex;
        }
        else
        {
            distanceBetweenSlots = -1 * (toSlotIndex - fromSlotIndex);
        }

        if (distanceBetweenSlots < 0)
        {
            distanceBetweenSlots = weaponSlots.Length + distanceBetweenSlots;
        }

        return distanceBetweenSlots;
    }
    void OnWeaponSwitched(Weapon newWeapon)
    {
        if (newWeapon == null) return;

        newWeapon.ShowWeapon(true);
    }



}
