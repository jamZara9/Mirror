using UnityEngine;

/// <summary>
/// 인벤토리에 추가할 수 있는 아이템의 인터페이스
/// </summary>
public interface IInventoryItem
{
    string ItemID { get; }
    IInventoryItemData ItemData { get; }
    Sprite Icon { get; }
    int Count { get; set;}
    int InventoryIndex { get; set; }
    int QuickSlotIndex { get; set; }
    bool IsActive { get; set; }
    bool IsUsable { get; set; }
    bool IsPickable { get; set; }

    GameObject ItemGameObject { get; }      // gameobject 반환을 위한 프로퍼티
}

/// <summary>
/// 인벤토리에 추가할 수 있는 아이템의 공통 데이터
/// </summary>
public interface IInventoryItemData{
    string Name { get; }
    string IconPath { get; }
    string Description { get; }
}
