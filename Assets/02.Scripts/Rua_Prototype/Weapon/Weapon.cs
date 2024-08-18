using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public enum WeaponAttackType
{
    Manual,
    Automatic,
}

public enum WeaponType
{
    Gun,
    Melee,
}

public abstract class Weapon : MonoBehaviour
{
    [Header("Information")]
    [Tooltip("The name that will be displayed in the UI for this weapon")]
    public string weaponName;
    public Sprite weaponIcon;
    public WeaponType weaponType;

    [Header("Attack Paarmeters")]
    [Tooltip("The type of weapon wil affect how it shoots")]
    public WeaponAttackType attackType;
    public float delayBetweenAttack = 0.5f;

    [Header("Internal References")]
    [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
    public GameObject WeaponRoot;

    [Header("Audio & Visual")]
    public GameObject impactVfx;
    public float impactVfxLifeTime;

    protected bool wantsToShoot = false;
    public UnityAction OnAttack;
    public event Action OnAttackProcessed;
    protected float lastTimeShot = Mathf.NegativeInfinity;
    public GameObject Owner { get; set; }
    public GameObject SourcePrefab { get; set; }
    public bool IsWeaponActive { get; private set; }

    public void ShowWeapon(bool show)
    {
        WeaponRoot.SetActive(show);

        IsWeaponActive = show;
    }

    public abstract bool HandleAttackInputs(bool inputDown, bool inputHeld, bool inputUp);
    public abstract bool TryAttack();
    public virtual void HandleAttack()
    {
        OnAttack?.Invoke();
        OnAttackProcessed?.Invoke();
    }
    // hit 가능한것만
    protected bool IsHitValid(RaycastHit hit)
    {
        Debug.Log(hit.collider.name + "1");
        // 충돌체 처리
        if (hit.collider.name == "")
        {

        }

        return true;
    }
    protected void OnHit(Vector3 normal, Vector3 point, Collider collider)
    {
        if (!impactVfx) return;

        GameObject impactVfxInstance = Instantiate(impactVfx, point + (normal * .1f), Quaternion.LookRotation(normal));

        if (impactVfxLifeTime > 0)
        {
            Destroy(impactVfxInstance, impactVfxLifeTime);
        }
    }
}
