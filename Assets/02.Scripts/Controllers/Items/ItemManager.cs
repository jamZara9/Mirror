using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 아이템을 관리하는 클래스
/// 
/// <para>
/// author: Argonaut
/// </para>
/// </summary>
public class ItemManager : MonoBehaviour
{
    [Header("Item Data")]
    // 아이템 데이터 딕셔너리 [ 아이템 ID, 아이템 데이터 ]
    public Dictionary<string, BaseItemData> itemDictionary = new Dictionary<string, BaseItemData>();   

    [Header("Item List")]
    // 필드(씬)에 존재하는 Item 오브젝트들을 저장하는 리스트
    public List<BaseItem> items = new List<BaseItem>();

    private string itemDataPath = "Json/items"; // 아이템 데이터 경로

    void Awake(){

        // 아이템 정보 로드
        LoadItemData(itemDataPath);
        
        // 모든 BaseItem 타입의 객체를 찾아서 리스트에 추가
        // BaseItem[] allBaseItems = Resources.FindObjectsOfTypeAll<BaseItem>();
        BaseItem[] allBaseItems = FindObjectsOfType<BaseItem>(); // 활성화된 오브젝트를 가져옴
        foreach (BaseItem item in allBaseItems){
            items.Add(item);                    // 리스트에 추가
            SetItemData(item);                  // 아이템 데이터 설정
            SetItemActiveState(item, true);     // 아이템 활성화
            SetItemIcon(item);                  // 아이템 아이콘 설정
            Debug.Log($"아이템 추가: {item.itemID}");
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
    /// <param name="itemPath">item 데이터가 있는 path</param>
    public void LoadItemData(string itemPath){
        string jsonData = FileManager.LoadJsonFile(itemDataPath);       // json 파일 로드
        if(jsonData != null){
            // json 데이터 -> 딕셔너리로 변환
            itemDictionary = JsonConvert.DeserializeObject<Dictionary<string, BaseItemData>>(jsonData);
            Debug.Log($"아이템 데이터 로드 완료. 아이템 개수: {itemDictionary.Count}");
        }
    }

    /// <summary>
    /// 아이템 아이콘 설정 함수
    /// </summary>
    private void SetItemIcon(BaseItem item){
        BaseItemData data = itemDictionary[item.itemID];
        try{
            if(data.iconPath == null || data.iconPath == ""){
                Debug.LogError("아이템 아이콘 경로가 비어있습니다.");
                return;
            }

            item.icon = Resources.Load<Sprite>(data.iconPath);
        }catch(System.Exception e){
            Debug.LogError($"아이템 아이콘 설정 중 오류 발생: {e}");
        }
    }

    /// <summary>
    /// 아이템 데이터 설정 함수
    /// </summary>
    /// <param name="item">설정하고자 하는 item</param>
    private void SetItemData(BaseItem item){
        BaseItemData data = itemDictionary[item.itemID];

        // 아이템 데이터 설정
        item.itemData = data;
    }

    /// <summary>
    /// 아이템 제거 함수
    /// </summary>
    /// <param name="item">제거할 아이템</param>
    public void RemoveItem(BaseItem item){
        items.Remove(item);
        Destroy(item.gameObject);
    }

    /// <summary>
    /// 아이템 제거 확인을 위한 테스트용 딜레이 함수
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator RemoveItemDelay(float delay){
        yield return new WaitForSeconds(delay);
        // RemoveItem(items.Find(x => x.itemID == testItemID));
    }

    /// <summary>
    /// 아이템 활성화 상태 설정 함수
    /// </summary>
    /// <param name="item">상태 변경이 일어날 아이템</param>
    /// <param name="state">상태</param>
    public void SetItemActiveState(BaseItem item, bool state){
        item.gameObject.SetActive(state);
        item.isActive = state;
    }

    /// <summary>
    /// 아이템 픽업 함수 [관측된 아이템을 플레이어 인벤토리에 추가]
    /// </summary>
    /// <param name="detectedItem">관측된 아이템의 BaseItem Script</param>
    /// <param name="playerInventory">아이템을 저장할 인벤토리</param>
    private void HandleItemPickup(BaseItem detectedItem, PlayerInventory playerInventory){
        // 감지된 아이템이 픽업 가능한지 확인
        if(detectedItem.isPickable){
            SetItemActiveState(detectedItem, false);           // 아이템 비활성화
            playerInventory.AddItem(detectedItem);            // 플레이어 인벤토리에 아이템 추가
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
            targetItem.UseItem();                        // 아이템 사용
            playerInventory.ChangeItemCount(targetItem);
            playerInventory.selectedItem = null;
            Debug.Log("아이템 사용");
        }
    }

    /// <summary>
    /// 아이템 이동 함수
    /// </summary>
    /// <param name="from">아이템 존재하는 위치</param>
    /// <param name="to">아이템을 이동시킬 위치</param>
    /// <param name="item">전달하고자 하는 아이템</param>
    public void TransferItem(IItemContainer from, IItemContainer to, BaseItem item){
        Debug.Log("아이템 이동");
        // @ TODO: 아이템 이동 로직 구현 아직 미완성
        from.RemoveItem(item);
        to.AddItem(item);
    }

}
