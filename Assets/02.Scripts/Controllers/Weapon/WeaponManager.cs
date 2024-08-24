using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 무기를 관리하는 클래스
/// </summary>
public class WeaponManager : BaseItemManager<BaseWeapon, BaseWeaponData>
{
     protected override string DataPath => "Json/weapons"; // 무기 데이터 경로

    /// <summary>
    /// 무기 데이터 설정 함수
    /// </summary>
    /// <param name="item">설정하고자 하는 item</param>
    protected override void SetItemData(BaseWeapon item)
    {
        if (dataDictionary.TryGetValue(item.weaponID, out var data))
        {
            item.weaponData = data;
        }
        else
        {
            Debug.LogWarning($"무기 데이터가 없습니다: {item.weaponID}");
        }
    }

    /// <summary>
    /// 아이템 아이콘 설정 함수
    /// </summary>
    protected override void SetItemIcon(BaseWeapon item)
    {
        if (dataDictionary.TryGetValue(item.weaponID, out var data) && !string.IsNullOrEmpty(data.iconPath))
        {
            item.icon = Resources.Load<Sprite>(data.iconPath);
        }
        else
        {
            Debug.LogError($"아이템 데이터가 없거나 아이콘 경로가 비어있습니다: {item.weaponID}");
        }
    }


    protected override string GetItemID(BaseWeapon item)
    {
        return item.weaponID;
    }


}
