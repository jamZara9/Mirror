using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponAttackType
{
    Manual,         // 수동 공격
    Automatic,      // 자동 공격
}

public enum WeaponType
{
    Gun,            // 총기
    Melee,          // 근접 무기
}

[System.Serializable]
public class BaseWeaponData : IInventoryItemData
{
    public string name;             // 무기 이름
    public string description;      // 무기 설명
    public string iconPath;         // 무기 아이콘의 경로
    public WeaponType type;         // 무기 타입
    public int count;               // 아이템 개수
    public int inventoryIndex;      // 인벤토리 인덱스
    public int quickSlotIndex;      // 퀵슬롯 인덱스
    public float damage;            // 데미지

    public string Name => name;
    public string Description => description;
    public string IconPath => iconPath;
}

public abstract class BaseWeapon : MonoBehaviour , IInventoryItem
{
    // IInventoryItem 인터페이스 구현
    public string ItemID => weaponID;
    public IInventoryItemData ItemData => weaponData;
    public Sprite Icon => icon;
    public int Count { get => weaponData.count; set => weaponData.count = value; }
    public int InventoryIndex { get => weaponData.inventoryIndex; set => weaponData.inventoryIndex = value; }
    public int QuickSlotIndex { get => weaponData.quickSlotIndex; set => weaponData.quickSlotIndex = value; }


    [Header("Information")]
    public string weaponID;            // 무기 ID
    public BaseWeaponData weaponData;  // 무기 데이터
    public WeaponAttackType attackType; // 공격 타입
    public float fireRate;              // 연사속도
    public float range;                 // 사정거리
    public Sprite icon;                 // 아이콘

    /// <summary>
    /// 공격할 때 호출되는 함수
    /// </summary>
    public abstract void Attack();
}
