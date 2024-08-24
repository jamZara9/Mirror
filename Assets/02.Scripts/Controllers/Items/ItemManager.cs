using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;

/// <summary>
/// 아이템을 관리하는 클래스
/// 
/// <para>
/// author: Argonaut
/// </para>
/// </summary>
public class ItemManager : BaseItemManager<BaseItem, BaseItemData>
{
    protected override string DataPath => "Json/items"; // 아이템 데이터 경로
    
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
    /// 아이템 이동 함수
    /// </summary>
    /// <param name="from">아이템 존재하는 위치</param>
    /// <param name="to">아이템을 이동시킬 위치</param>
    /// <param name="item">전달하고자 하는 아이템</param>
    public void TransferItem(IItemContainer from, IItemContainer to, BaseItem item){
        // @ TODO: 아이템 이동 로직 구현 아직 미완성

        if(from != null && to != null){
            from.RemoveItem(item);
            to.AddItem(item);
            Debug.Log("아이템 이동");
        }else{
            Debug.LogError("아이템 이동 실패");
        }
    }

}
