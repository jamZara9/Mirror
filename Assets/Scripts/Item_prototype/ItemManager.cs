using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 아이템을 관리하는 클래스
/// </summary>
public class ItemManager : MonoBehaviour
{
    private static ItemManager _itemManager;              // 싱글톤 인스턴스

    // public static ItemManager Instance => _itemManager;   // 인스턴스 반환
    // 싱글톤 진행
    public static ItemManager Instance {
        get{
            if(_itemManager == null){
                // 씬에서 ItemManager를 찾음
                _itemManager = FindObjectOfType<ItemManager>();

                // 씬에 ItemManager가 없는 경우 새로운 GameObject를 생성하여 추가
                if (_itemManager == null)
                {
                    GameObject singleton = new GameObject("ItemManager");
                    _itemManager = singleton.AddComponent<ItemManager>();
                }
            }
            return _itemManager;
        }
    }


    [Header("Item Data")]
    // 아이템 데이터 딕셔너리 [ 아이템 ID, 아이템 데이터 ]
    public Dictionary<string, BaseItemData> itemDictionary;   


    [Header("Item List")]
    // 필드에 존재하는 Item 오브젝트들을 저장하는 리스트
    public List<BaseItem> items;


    [Header("Item Interaction")]
    public GameObject detectedItem = null;    // 감지된 아이템
    public GameObject choiceItem = null;      // 선택된 아이템

    // 테스트옹 변수들
    private string testItemID = "Item001";

    
    void Awake(){
        // 싱글톤 인스턴스 설정
        if(_itemManager == null){
            _itemManager = this;                            // 인스턴스 할당
            DontDestroyOnLoad(gameObject);                  // 씬 전환 시에도 파괴되지 않도록 설정
        }
        else{
            Destroy(gameObject);
        }

        // 아이템 정보 로드
        LoadItemData();
        
        // 모든 BaseItem 타입의 객체를 찾아서 리스트에 추가
        // BaseItem[] allBaseItems = Resources.FindObjectsOfTypeAll<BaseItem>();
        BaseItem[] allBaseItems = FindObjectsOfType<BaseItem>(); // 활성화된 오브젝트를 가져옴
        foreach (BaseItem item in allBaseItems){
            items.Add(item);                    // 리스트에 추가
            setItemData(item);                  // 아이템 데이터 설정
            SetItemActiveState(item, true);     // 아이템 활성화
            Debug.Log($"아이템 추가: {item.itemID}");
        }

        // CreateItem(testItemID);

        // // 아이템 제거 테스트
        // StartCoroutine(RemoveItemDelay(3.0f));
    }

    void LoadItemData(){
        string jsonFilePath = Path.Combine(Application.dataPath, "Scripts/Item_prototype/Json/items.json");

        if(File.Exists(jsonFilePath)){

            string jsonData = FileManager.LoadJsonFile(jsonFilePath);       // json 파일 로드

            // json 데이터 -> 딕셔너리로 변환
            itemDictionary = JsonConvert.DeserializeObject<Dictionary<string, BaseItemData>>(jsonData);
            //Debug.Log($"아이템 데이터 로드 완료. 아이템 개수: {itemDictionary.Count}");
            
            // // itemDictionary의 모든 키와 값 출력
            // foreach (var kvp in itemDictionary)
            // {
            //     Debug.Log($"Key: {kvp.Key}, Name: {kvp.Value.name}, Description: {kvp.Value.description}, Type: {kvp.Value.type}, IconPath: {kvp.Value.iconPath}");
            // }
        }
        else{
            Debug.LogError($"JSON 파일을 찾을 수 없습니다. 경로: {jsonFilePath}");
        }
    }

    /// <summary>
    /// 아이템 데이터 설정 함수
    /// </summary>
    /// <param name="item">설정하고자 하는 item</param>
    private void setItemData(BaseItem item){
        BaseItemData data = itemDictionary[item.itemID];

        // 아이템 데이터 설정
        item.itemData = data;
    }

    /// <summary>
    /// 아이템 생성 함수
    /// </summary>
    /// <param name="itemID">생성하고자 하는 Item의 ID</param>
    public void CreateItem(string itemID){

        string itemPrefabPath = "Prefabs/Items/"; // 아이템 프리팹 경로

        // 아이템 프리팹 로드
        GameObject itemPrefab = Resources.Load<GameObject>(itemPrefabPath + itemID);
        if (itemPrefab == null)
        {
            Debug.LogError("아이템 프리팹을 찾을 수 없습니다: " + itemPrefabPath + itemID);
            return;
        }

        // 아이템 생성
        GameObject itemObject = Instantiate(itemPrefab);
        BaseItem item = itemObject.GetComponent<BaseItem>();

        // 추후 위치에 대한 값들이 결정되었을 경우 위치 설정 필요
        if(item != null){
            item.itemID = itemID;
            item.transform.position = new Vector3(0, 0, 0);
            item.transform.rotation = Quaternion.identity;
            item.transform.localScale = new Vector3(1, 1, 1);
            item.gameObject.SetActive(true);

            // 아이템 데이터 설정
            setItemData(item);

            // 리스트에 추가
            items.Add(item);
        }else{
            Debug.LogError("BaseItem을 찾을 수 없습니다.");
        }

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
        RemoveItem(items.Find(x => x.itemID == testItemID));
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
    /// 아이템 줍기 함수
    /// </summary>
    public void PickupItem(){
        // 감지된 아이템이 픽업 가능한지 확인
        if(detectedItem.GetComponent<BaseItem>().isPickable){
            // detectedItem.GetComponent<BaseItem>().Deactivate();  // 아이템 비활성화

            PlayerInventory.Instance.AddItem(detectedItem.GetComponent<BaseItem>());    // 아이템 추가
            SetItemActiveState(detectedItem.GetComponent<BaseItem>(), false);           // 아이템 비활성화
            Debug.Log("아이템 줍기");

            choiceItem = detectedItem;  // 선택된 아이템 설정  [임시]
        }
    }

    public void UseItem(){
        // 선택된 아이템이 사용 가능한지 확인
        // 추후 useable 여부에 대한 확인이 필요
        if(choiceItem != null){
            choiceItem.GetComponent<BaseItem>().UseItem();  // 아이템 사용
            Debug.Log("아이템 사용");
        }
    }

}
