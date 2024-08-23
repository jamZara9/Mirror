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
}
