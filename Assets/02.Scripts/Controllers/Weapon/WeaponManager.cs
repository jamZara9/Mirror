using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 무기를 관리하는 클래스
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Data")]
    // 무기 데이터 딕셔너리 [ 무기 ID, 무기 데이터 ]
    public Dictionary<string, BaseWeaponData> weaponDictionary = new Dictionary<string, BaseWeaponData>();

    [Header("Weapon List")]
    // 필드(씬)에 존재하는 Weapon 오브젝트들을 저장하는 리스트
    public List<BaseWeapon> weapons = new List<BaseWeapon>();

    private string weaponDataPath = "Json/weapons"; // 무기 데이터 경로

    void Awake()
    {

        // 무기 정보 로드
        LoadItemData(weaponDataPath);

        // 모든 BaseWeapon 타입의 객체를 찾아서 리스트에 추가
        // BaseWeapon[] allBaseWeapons = Resources.FindObjectsOfTypeAll<BaseWeapon>();
        BaseWeapon[] allBaseWeapons = FindObjectsOfType<BaseWeapon>(); // 활성화된 오브젝트를 가져옴
        foreach (BaseWeapon weapon in allBaseWeapons)
        {
            weapons.Add(weapon);                    // 리스트에 추가
            SetItemData(weapon);                  // 무기 데이터 설정
            // SetWeaponActiveState(weapon, true);     // 무기 활성화
            SetItemIcon(weapon);                  // 무기 아이콘 설정
            Debug.Log($"무기 추가: {weapon.weaponID}");
        }
    }

    /// <summary>
    /// 무기 데이터 로드 함수
    /// </summary>
    /// <param name="itemPath">item 데이터가 있는 path</param>
    public void LoadItemData(string itemPath)
    {
        string jsonData = FileManager.LoadJsonFile(weaponDataPath);       // json 파일 로드
        if (jsonData != null)
        {
            // json 데이터 -> 딕셔너리로 변환
            weaponDictionary = JsonConvert.DeserializeObject<Dictionary<string, BaseWeaponData>>(jsonData);
            Debug.Log($"아이템 데이터 로드 완료. 아이템 개수: {weaponDictionary.Count}");
        }
    }

    /// <summary>
    /// 무기 데이터 설정 함수
    /// </summary>
    /// <param name="item">설정하고자 하는 item</param>
    private void SetItemData(BaseWeapon item)
    {
        BaseWeaponData data = weaponDictionary[item.weaponID];

        // 아이템 데이터 설정
        item.weaponData = data;
    }

    /// <summary>
    /// 아이템 아이콘 설정 함수
    /// </summary>
    private void SetItemIcon(BaseWeapon item){
        BaseWeaponData data = weaponDictionary[item.weaponID];
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


}
