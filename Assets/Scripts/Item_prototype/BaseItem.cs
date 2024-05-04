using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Quest,          // 퀘스트 아이템
    Immediate,      // 즉시 아이템
    Persistent,     // 지속 아이템
    Ammunition,     // 탄약 아이템
}

[System.Serializable]
public class BaseItemData
{
    [Tooltip("The name that will be displayed in the UI for this item")]
    public string name;             // 아이템 이름
    public string iconPath;         // 아이템 아이콘의 경로
    public ItemType type;           // 아이템 타입
    public string description;      // 아이템 설명
}

public abstract class BaseItem : MonoBehaviour
{
    [Header("Information")]
    public string itemID;               // 아이템 ID
    public BaseItemData itemData;       // 아이템 데이터

    [Header("Audio & Visual")]
    public GameObject impactVFX;
    public float impactVFXLifeTime;     // 지속시간

    [Header("Item Parameters")]
    public bool isActive = false;       // 활성화 여부
    public bool isUsable = false;       // 사용 가능 여부
    public string pickupArea;           // 픽업 가능한 영역

    /// <summary>
    /// 아이템을 사용할 때 호출되는 함수
    /// </summary>
    public abstract void UseItem();

    /// <summary>
    /// 아이템 활성화 함수
    /// </summary>
    public virtual void Activate()
    {
        isActive = true;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 아이템 비활성화 함수
    /// </summary>
    public virtual void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}
