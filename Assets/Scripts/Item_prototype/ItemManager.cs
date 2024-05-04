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
    public static ItemManager Instance => _itemManager;   // 인스턴스 접근용 프로퍼티

    // json 파일 경로
    public string jsonFilePath= Path.Combine(Application.dataPath, "Scripts/Item_prototype/Json/items.json");

    // 아이템 데이터 딕셔너리 [ 아이템 ID, 아이템 데이터 ]
    public Dictionary<string, BaseItemData> itemDictionary;   

    // 필드에 생성된 Item 오브젝트들을 저장하는 리스트
    public List<BaseItem> items;
    
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
        InitializeItem();
        
        // 모든 BaseItem 타입의 객체를 찾아서 리스트에 추가
        BaseItem[] allBaseItems = Resources.FindObjectsOfTypeAll<BaseItem>();
        foreach (BaseItem item in allBaseItems){
            items.Add(item);        // 리스트에 추가
            setItemData(item);      // 아이템 데이터 설정
            ShowItem(item, true);   // 아이템 활성화
        }

        
    }

    void InitializeItem(){
        if(File.Exists(jsonFilePath)){
            // json 데이터 로드
            string jsonData = FileManager.LoadJsonFile(jsonFilePath);

            // json 데이터 -> 딕셔너리로 변환
            itemDictionary = JsonConvert.DeserializeObject<Dictionary<string, BaseItemData>>(jsonData);
            Debug.Log($"아이템 데이터 로드 완료. 아이템 개수: {itemDictionary.Count}");
            
            // itemDictionary의 모든 키와 값 출력
            foreach (var kvp in itemDictionary)
            {
                Debug.Log($"Key: {kvp.Key}, Name: {kvp.Value.name}, Description: {kvp.Value.description}, Type: {kvp.Value.type}, IconPath: {kvp.Value.iconPath}");
            }
        }
        else{
            Debug.LogError($"JSON 파일을 찾을 수 없습니다. 경로: {jsonFilePath}");
        }
    }

    /// <summary>
    /// 아이템 활성화 여부를 설정하는 함수
    /// </summary>
    /// <param name="show"></param>
    public void ShowItem(BaseItem item, bool show){
        if(show){
            item.Activate();
        }
        else{
            item.Deactivate();
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
}
