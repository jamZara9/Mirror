using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Quest,          // 퀘스트 아이템
    Immediate,      // 즉시 아이템
    Persistent,     // 지속 아이템
    Ammunition,     // 탄약 아이템
    END             // 종료
}

[System.Serializable]
public class BaseItemData : IInventoryItemData
{
    [Tooltip("The name that will be displayed in the UI for this item")]
    public string name;             // 아이템 이름
    public string description;      // 아이템 설명
    public string iconPath;         // 아이템 아이콘의 경로
    public ItemType type;           // 아이템 타입
    public int count;               // 아이템 개수
    public int invenSpace;          // 인벤토리 공간
    public int inventoryIndex;      // 인벤토리 인덱스
    public int quickSlotIndex;      // 퀵슬롯 인덱스

    public string Name => name;
    public string IconPath => iconPath;
    public string Description => description;
}

public abstract class BaseItem : MonoBehaviour, IInventoryItem
{
    // IInventoryItem 인터페이스 구현
    public string ItemID => itemID;
    public IInventoryItemData ItemData => itemData;
    public Sprite Icon => icon;
    public int Count { get => itemData.count; set => itemData.count = value; }
    public int InventoryIndex { get => itemData.inventoryIndex; set => itemData.inventoryIndex = value; }
    public int QuickSlotIndex { get => itemData.quickSlotIndex; set => itemData.quickSlotIndex = value; }
    public GameObject ItemGameObject => this.gameObject;
    public bool IsActive { get => isActive; set => isActive = value; }
    public bool IsUsable { get => isUsable; set => isUsable = value; }
    public bool IsPickable { get => isPickable; set => isPickable = value; }


    [Header("Information")]
    public string itemID;               // 아이템 ID
    public BaseItemData itemData;       // 아이템 데이터

    [Header("Audio & Visual")]
    public GameObject impactVFX;
    public float impactVFXLifeTime;     // 지속시간
    public GameObject particlePos;      // 파티클 위치
    public Sprite icon;                  // 아이콘

    // 추후 getter / setter로 변경
    [Header("Item Parameters")]
    public bool isActive = false;       // 활성화 여부
    public bool isUsable = false;       // 사용 가능 여부
    public bool isPickable = false;     // 줍기 가능 여부
    public string pickupArea;           // 픽업 가능한 영역

    /// <summary>
    /// 아이템을 사용할 때 호출되는 함수
    /// </summary>
    public abstract void UseItem();

    public void UseEffect(){
        // 아이템 사용 효과
        if(impactVFX != null){
            GameObject impactVFXObject = Instantiate(impactVFX, particlePos.transform.position, Quaternion.identity);
            Destroy(impactVFXObject, impactVFXLifeTime);
        }
    }
}
