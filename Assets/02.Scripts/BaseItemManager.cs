using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public abstract class BaseItemManager<T,D> :  MonoBehaviour where T : MonoBehaviour
{
    [Header("Item Data")]
    public Dictionary<string, D> dataDictionary = new();    // 데이터 딕셔너리 [ 아이템 ID, 아이템 데이터 ]

    [Header("Item List")]
    public List<T> items = new();                           // 필드(씬)에 존재하는 Item 오브젝트들을 저장하는 리스트

    protected abstract string DataPath { get; }             // 아이템 데이터 경로
    
    protected virtual void Awake(){
        LoadItemData(DataPath);
        InitializeItems();
    }

    /// <summary>
    /// 아이템 초기화 함수
    /// </summary>
    private void InitializeItems()
    {
        T[] allItems = FindObjectsOfType<T>();
        foreach (T item in allItems)
        {
            items.Add(item);
            SetItemData(item);
            SetItemIcon(item);
            Debug.Log($"{typeof(T).Name} 추가: {GetItemID(item)}");
        }
    }

    // EventManager에 이벤트 핸들러 등록
    private void OnEnable() {
        EventManager.OnItemPickup += HandleItemPickup;
        EventManager.OnItemUse += HandleItemUse;
    }

    // EventManager에 이벤트 핸들러 해제
    private void OnDisable() {
        EventManager.OnItemPickup -= HandleItemPickup;
        EventManager.OnItemUse -= HandleItemUse;
    }
    
    /// <summary>
    /// 아이템 데이터 로드 함수
    /// </summary>
    /// <param name="itemPath"></param>
    protected void LoadItemData(string itemPath){
        string jsonData = FileManager.LoadJsonFile(itemPath);
        if(jsonData != null){
            dataDictionary = JsonConvert.DeserializeObject<Dictionary<string, D>>(jsonData);

            Type type = items.GetType();
            Debug.Log($"{type.GetGenericArguments()[0]} 데이터 로드 완료. 아이템 개수: {dataDictionary.Count}");
        }
    }

    protected abstract void SetItemData(T item);    // 아이템 데이터 설정 함수
    protected abstract void SetItemIcon(T item);    // 아이템 아이콘 설정 함수
    protected abstract string GetItemID(T item);    // 아이템 ID 반환 함수

    /// <summary>
    /// 아이템 활성화 상태 설정 함수
    /// </summary>
    /// <param name="item">상태 변경이 일어날 아이템</param>
    /// <param name="state">상태</param>
    protected void SetItemActiveState(IInventoryItem item, bool state){
        item.ItemGameObject.SetActive(state);
        item.IsActive = state;
    }

    /// <summary>
    /// 아이템 픽업 함수 [관측된 아이템을 플레이어 인벤토리에 추가]
    /// </summary>
    /// <param name="detectedItem">관측된 아이템</param>
    /// <param name="playerInventory">아이템을 저장할 인벤토리</param>
    private void HandleItemPickup(IInventoryItem detectedItem, PlayerInventory playerInventory){
        if(detectedItem.IsPickable){
            Debug.Log(detectedItem.ItemData.Name);
            SetItemActiveState(detectedItem, false);          // 아이템 비활성화
            playerInventory.AddItem(detectedItem);            // 플레이어 인벤토리에 아이템 추가
            Debug.Log("아이템 획득");
        }
    }

    /// <summary>
    /// 아이템 사용 함수
    /// </summary>
    /// <param name="playerInventory">아이템을 사용할 인벤토리</param>
    private void HandleItemUse(PlayerInventory playerInventory){
        // 선택된 아이템이 사용 가능한지 확인
        // @TODO: useable 여부에 대한 확인이 필요
        if(playerInventory.selectedItem != null){
            BaseItem targetItem = playerInventory.selectedItem.GetComponent<BaseItem>();
            if(targetItem.Count > 0){
                targetItem.UseItem();                        // 아이템 사용
                targetItem.Count -= 1;                       // 아이템 개수 감소
                playerInventory.selectedItem = null;
                Debug.Log("아이템 사용");
            }
        }
    }
}
