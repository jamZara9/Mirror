using System.Collections;
using UnityEngine;

/// <summary>
/// 전체 아이템(소모성, 무기 등)을 관리하는 클래스 
/// </summary>
public class ItemManager : BaseItemManager<BaseItem, BaseItemData>, IManager
{
    protected override string DataPath => "Json/items"; // 아이템 데이터 경로

    [SerializeField] private GameObject itemGroup;   // 아이템 그룹

    public void Initialize(string sceneName)
    {
        if (itemGroup == null)
        {
            itemGroup = GameObject.Find("ItemGroup");
        }

        // itemGroup의 자식 요소를 모두 활성화
        for (int i = 0; i < itemGroup.transform.childCount; i++)
        {
            itemGroup.transform.GetChild(i).gameObject.SetActive(true);
            itemGroup.transform.GetChild(i).gameObject.GetComponent<IInventoryItem>().IsActive = true;
            itemGroup.transform.GetChild(i).gameObject.GetComponent<IInventoryItem>().IsPickable = true;
            itemGroup.transform.GetChild(i).gameObject.GetComponent<IInventoryItem>().IsUsable = true;
        }
    }

    /// <summary>
    /// 아이템 데이터 설정 함수
    /// </summary>
    protected override void SetItemData(BaseItem item)
    {
        if (dataDictionary.TryGetValue(item.itemID, out var data))
        {
            item.itemData = data;
        }
        else
        {
            Debug.LogWarning($"아이템 데이터가 없습니다: {item.itemID}");
        }
    }

    /// <summary>
    /// 아이템 아이콘 설정 함수
    /// </summary>
    protected override void SetItemIcon(BaseItem item)
    {
        if (dataDictionary.TryGetValue(item.itemID, out var data) && !string.IsNullOrEmpty(data.iconPath))
        {
            item.icon = Resources.Load<Sprite>(data.iconPath);
        }
        else
        {
            Debug.LogError($"아이템 데이터가 없거나 아이콘 경로가 비어있습니다: {item.itemID}");
        }
    }

    protected override string GetItemID(BaseItem item)
    {
        return item.itemID;
    }


    // Object Pooling을 이용한 Item 생성
    // SystemManager를 이용한 Save & Load
    
}
