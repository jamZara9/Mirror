using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


public class Gun : Weapon
{
    [Header("Internal References")]
    public Transform weaponMuzzle;
    [Header("Ammo Parameters")]
    public bool automaticReload = true;
    public bool hasPhysicalBullets = false;
    public int clipSize = 30;
    public GameObject shellCasing;
    public Transform ejectionPort;
    [Range(0f, 5f)] public float shellCasingEjectionForce = 2.0f;
    [Range(1, 30)] public int shellPoolSize = 1;
    public float ammoReloadDelay = 2f;
    public int maxAmmo = 8;
    public int invAmmo = 300;

    [Header("Shoot Parameters")]
    public float bulletSpreadAngle = 0f;
    public int bulletsPerShot = 1;
    [Range(0f, 2f)] public float recoilForce = 1;
    [Range(0f, 1f)] public float aimZoomRatio = 1f;
    public Vector3 aimOffset;

    [Header("Audio & Visual")]
    [Tooltip("Prefab of the muzzle flash")]
    public GameObject muzzleFlashPrefab;
    [Tooltip("Unparent the muzzle flash instance on spawn")]
    public bool unParentMuzzleFlash;

    int carriedPhysicalBullets;
    int currentAmmo;
    Vector3 lastMuzzlePosition;
    public float CurrentAmmoRatio { get; private set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }
    public float GetAmmoNeededToShoot() => 1f / (maxAmmo * bulletsPerShot);
    public int GetCarriedPhysicalBullets() => carriedPhysicalBullets;
    public int GetCurrentAmmo() => currentAmmo;
    public bool IsReloading { get; private set; }

    private void Awake()
    {
        currentAmmo = maxAmmo;
        carriedPhysicalBullets = hasPhysicalBullets ? clipSize : 0;
        lastMuzzlePosition = weaponMuzzle.position;

        weaponType = WeaponType.Gun;
    }


    public void AddCarriablePhysicalBullets(int count) => carriedPhysicalBullets =
        Mathf.Max(carriedPhysicalBullets + count, maxAmmo);
    void Reload()
    {
        IsReloading = false;

        int requireAmmoCnt = maxAmmo - currentAmmo;
        int reloadAmmoCnt;

        if (invAmmo >= requireAmmoCnt)
        {
            reloadAmmoCnt = requireAmmoCnt;
            invAmmo -= requireAmmoCnt;
        }
        else
        {
            reloadAmmoCnt = invAmmo;
            invAmmo = 0;
        }


        currentAmmo += reloadAmmoCnt;

        CurrentAmmoRatio = currentAmmo / maxAmmo;
    }

    private void Update()
    {
        UpdateAmmo();

        if (Time.deltaTime <= 0) return;

        MuzzleWorldVelocity = (weaponMuzzle.position - lastMuzzlePosition) / Time.deltaTime;
        lastMuzzlePosition = weaponMuzzle.position;
    }
    void UpdateAmmo()
    {
        if (!IsReloading && automaticReload && lastTimeShot + ammoReloadDelay < Time.time && currentAmmo < maxAmmo)
        {
            Debug.Log("reload");
            StartReloadAnimation();
        }

        if (!IsReloading && currentAmmo <= 0)
        {
            StartReloadAnimation();
        }

        CurrentAmmoRatio = (float)currentAmmo / maxAmmo;
    }
    public void StartReloadAnimation()
    {
        if (invAmmo <= 0) return;

        Invoke(nameof(Reload), ammoReloadDelay);
        // process animation
        IsReloading = true;
    }

    public void UseAmmo(int amount)
    {
        currentAmmo = Math.Clamp(currentAmmo - amount, 0, maxAmmo);
        carriedPhysicalBullets -= amount;
        carriedPhysicalBullets = Math.Clamp(carriedPhysicalBullets, 0, maxAmmo);
        lastTimeShot = Time.time;
    }

    public override bool HandleAttackInputs(bool inputDown, bool inputHeld, bool inputUp)
    {
        wantsToShoot = inputDown || inputHeld;
        switch (attackType)
        {
            case WeaponAttackType.Manual:
                if (inputDown) return TryAttack();

                return false;
            case WeaponAttackType.Automatic:
                if (inputHeld) return TryAttack();

                return false;

            default:
                return false;
        }
    }
    public override bool TryAttack()
    {
        if (currentAmmo >= 1 && lastTimeShot + delayBetweenAttack < Time.time)
        {
            HandleAttack();
            currentAmmo -= 1;

            return true;
        }
        return false;
    }

    public override void HandleAttack()
    {
        int bulletsPerShotFinal = bulletsPerShot;

        // spawn all bullets with random direction
        for (int i = 0; i < bulletsPerShotFinal; i++)
        {
            Vector3 shotDirection = GetShotDirectionWithInSpread(weaponMuzzle);
            Debug.DrawRay(weaponMuzzle.position, weaponMuzzle.forward * 100f, Color.red);
            if (Physics.Raycast(weaponMuzzle.position, weaponMuzzle.forward, out RaycastHit hit, 100f))
            {
                if (IsHitValid(hit)) OnHit(hit.normal, hit.point, hit.collider);
            }
        }

        // muzzle flash
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position,
            weaponMuzzle.rotation, weaponMuzzle.transform);

            if (unParentMuzzleFlash) muzzleFlashInstance.transform.SetParent(null);

            Destroy(muzzleFlashInstance, 2f);
        }

        lastTimeShot = Time.time;
        base.HandleAttack();
    }


    public Vector3 GetShotDirectionWithInSpread(Transform shootTransform)
    {
        float spreadAngleRatio = bulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
        spreadAngleRatio);

        return spreadWorldDirection;
    }

    // public override void Attack()
    // {
    //     if (Time.time > nextShotTime)
    //     {
    //         nextShotTime = Time.time + msBetweenShots / 1000f;
    //         Debug.DrawRay(weaponMuzzle.position, weaponMuzzle.forward * 100f, Color.red);

    //         if (Physics.Raycast(weaponMuzzle.position, weaponMuzzle.forward, out RaycastHit hit, 100f))
    //         {
    //             Debug.Log(hit.collider.name + "1");
    //             // 충돌체 처리
    //             if (hit.collider.name == "")
    //             {

    //             }
    //         }
    //     }
    // }
}
