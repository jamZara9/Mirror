using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

/// <summary>
/// UI 상수를 관리하는 클래스
/// </summary>
public class UIConstants
{
    public const string UIGroup = "________UI__________";
    public const string HUDCanvas = "HUD_Canvas";
    public const string InventoryCanvas = "Inventory_Canvas";
    public const string QuickSlotCanvas = "QuickSlot_Canvas";

    public const string HP = "HP";
    public const string AttackDamage = "AttackDamage";
    public const string AttackSpeed = "AttackSpeed";
    public const string AttackRange = "AttackRange";
    public const string WalkSpeed = "WalkSpeed";

    public const string QuickSlot = "QuickSlot";
}

public enum ECanvas
{
    HUD, INVENTORY, QUICKSLOT, END
}

/// <summary>
/// UI를 관리하는 클래스
/// </summary>
public class UIManager : IManager
{
    // UI 프리팹 경로를 저장하는 Dictionary
    private Dictionary<Type, string> _uiPrefabPaths  = new()
    {
        { typeof(VideoCanvas), "Prefabs/UI/VideoCanvas" },
    };

    // UI 프리팹을 저장하는 Dictionary
    private Dictionary<Type, GameObject> _uiPrefabs = new();

    /// <summary>
    /// UI의 사용이 종료되었을 때 호출되는 함수
    /// </summary>
    /// <param name="uiObject">사용을 종료할 UI</param>
    public void UIFinished(GameObject uiObject){
        Debug.Log("UI Finished");
        uiObject.SetActive(false);
    }

    // UI 오브젝트를 가져오거나 없으면 생성
    public T GetOrAddUI<T>() where T : Component
    {
        Type type = typeof(T);

        // 이미 인스턴스화된 UI가 있는지 확인
        if (_uiPrefabs.TryGetValue(type, out GameObject uiObject))
        {
            return uiObject.GetComponent<T>();
        }

        // 프리팹 경로가 있는지 확인하고 로드
        if (_uiPrefabPaths.TryGetValue(type, out string path))
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"프리팹 경로에 해당하는 오브젝트가 없습니다: {path}");
                return null;
            }

            // UI 프리팹을 인스턴스화하고 저장
            uiObject = GameObject.Instantiate(prefab);
            _uiPrefabs[type] = uiObject;
            return uiObject.GetComponent<T>();
        }
        else
        {
            Debug.LogError($"UI 프리팹 경로가 정의되지 않았습니다: {type}");
            return null;
        }
    }

    // IManager 인터페이스 구현
    public void Initialize(string sceneName)
    {
        if(sceneName == SceneConstants.StartScene){

        }

        if(sceneName == SceneConstants.PlaygroundA){
            // Find_Canvas();
            
            // SetUiDictionary(_UIGroup);

            // foreach (var canvas in _canvasDictionary)
            // {
            //     if(canvas.Key == UIConstants.HUDCanvas)
            //     {
            //         canvas.Value.gameObject.SetActive(true);
            //     }else{
            //         canvas.Value.gameObject.SetActive(false);
            //     }
            // }

            // _txtHP.text = "HP : " + GameManager.Instance.playerStatus.CurrentHealth + " / 100";    // (text) 초기 세팅
            // _txtAttackDamage.text = "Attack Damage : " + GameManager.Instance.playerStatus.settings.attackDamage;    // (text) 초기 세팅
            // _txtAttackSpeed.text = "Attack Speed : " + GameManager.Instance.playerStatus.settings.attackDelay;    // (text) 초기 세팅
            // _txtAttackRange.text = "Attack Range : " + GameManager.Instance.playerStatus.settings.attackRange;    // (text) 초기 세팅
            // _txtWalkSpeed.text = "Walk Speed : " + GameManager.Instance.playerStatus.settings.walkSpeed;    // (text) 초기 세팅
        }
        
    }




    #region refactoring before
    [SerializeField]
    private Canvas HUD_Canvas;
    [SerializeField]
    private Canvas Inventory_Canvas;
    [SerializeField]
    private Canvas QuickSlot_Canvas;

    public GameObject Text_ItemName;
    public GameObject Text_ItemDescription;

    [SerializeField] private TextMeshProUGUI _txtHP;                // HP 텍스트 (test용)
    [SerializeField] private TextMeshProUGUI _txtAttackDamage;      // 공격력 텍스트 (test용)
    [SerializeField] private TextMeshProUGUI _txtAttackSpeed;       // 공격속도 텍스트 (test용)
    [SerializeField] private TextMeshProUGUI _txtAttackRange;       // 방어력 텍스트 (test용)
    [SerializeField] private TextMeshProUGUI _txtWalkSpeed;         // 이동속도 텍스트 (test용)

    [SerializeField] private GameObject _UIGroup;   // UI를 담고 있는 그룹
    // Test
    private Dictionary<string, Canvas> _canvasDictionary  = new();  // UI 캠버스 딕셔너리
    private Dictionary<string, GameObject> _uiDictionary = new();  // UI 딕셔너리

    /// <summary>
    /// 모든 UI를 canvas단위로 나눠 관리할 map인데 실 사용 여부는 좀 더 고민해봐야 합니다
    /// </summary>
    // private List<Dictionary<string, GameObject>> _uiDictionary = new((int)ECanvas.END); //모든 UI를 canvas단위로 나눠 관리할 딕셔너리    

    /// <summary>
    /// UI를 담고 있는 그룹을 찾아 딕셔너리에 추가하는 함수
    /// </summary>
    /// <param name="ui"></param>
    private void SetUiDictionary(GameObject ui)
    {
        if (ui == null) return;

        // 큐를 사용하여 계층 구조를 순회합니다.
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(ui.transform);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();

            // 현재 Transform의 자식들을 큐에 추가합니다.
            for (int i = 0; i < current.childCount; i++)
            {
                Transform child = current.GetChild(i);
                queue.Enqueue(child);
                _uiDictionary[child.gameObject.name] = child.gameObject;
                // Debug.Log($"{child.gameObject.name} : {child.gameObject}");
            }
        }
    }

    /// <summary>
    /// UI를 담고 있는 그룹을 찾는 함수
    /// </summary>
    public void Find_Canvas()
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////// Test
        if (_UIGroup == null)
        {
            _UIGroup = GameObject.Find(UIConstants.UIGroup);
        }

        // uiGroup의 자식 요소를 모두 활성화
        // @TODO: 추후 딕셔너리로 가지고 다니면 어떨지 고민 필요
        for (int i = 0; i < _UIGroup.transform.childCount; i++)
        {
            Debug.Log(_UIGroup.transform.GetChild(i).name);
            
            var child = _UIGroup.transform.GetChild(i).gameObject;      // 자식 요소를 가져옴
            if(child.TryGetComponent<Canvas>(out var canvas))
            {
                _canvasDictionary[child.name] = canvas;
                Debug.Log($"{child.name} : {canvas}");
            }

            child.SetActive(true);

            switch(_UIGroup.transform.GetChild(i).name)
            {
                case UIConstants.HUDCanvas:
                    HUD_Canvas = _canvasDictionary[UIConstants.HUDCanvas];
                    break;
                case UIConstants.InventoryCanvas:
                    Inventory_Canvas = _canvasDictionary[UIConstants.InventoryCanvas];
                    break;
                case UIConstants.QuickSlotCanvas:
                    QuickSlot_Canvas = _canvasDictionary[UIConstants.QuickSlotCanvas];
                    break;
            }
            
            // _UIGroup.transform.GetChild(i).gameObject.SetActive(true);
        }
    }


    //-------------------------------HUD-------------------------------------//
    #region HUD
    public Canvas HUD()
    {
        return HUD_Canvas;
    }

    public void HUD_Active(bool Active)
    {
        HUD_Canvas.gameObject.SetActive(Active);
    }
    #endregion


    //----------------------------Inventory-----------------------------------//
    #region Inventory
    public Canvas Inventory()
    {
        return Inventory_Canvas;
    }

    public void Inventory_Active(bool Active)
    {
        Inventory_Canvas.gameObject.SetActive(Active);
    }

    #endregion


    //-----------------------------QuickSlot-----------------------------------//
    #region QuickSlot
    public Canvas QuickSlot()
    {
        return QuickSlot_Canvas;
    }


    public void QuickSlot_Active(bool Active)
    {
        QuickSlot_Canvas.gameObject.SetActive(Active);
    }

    #endregion

    #region Test
    /// <summary>
    /// Canvas를 활성화/비활성화 하는 함수
    /// </summary>
    /// <param name="canvasName">캔버스 이름</param>
    /// <param name="isActive"></param>
    public void SetCanvasActive(string canvasName, bool isActive)
    {
        if(_canvasDictionary.TryGetValue(canvasName, out var canvas))
        {
            canvas.gameObject.SetActive(isActive);
        }else{
            Debug.LogError($"에러 발생: {canvasName}은 존재하지 않는 Canvas 이름입니다.");
        }
    }

    public Canvas GetCanvas(string canvasName){
        if(_canvasDictionary.TryGetValue(canvasName, out var canvas))
        {
            return canvas;
        }else{
            Debug.LogError($"에러 발생: {canvasName}은 존재하지 않는 Canvas 이름입니다.");
            return null;
        }
    }
    #endregion

    /// <summary>
    /// UI 텍스트 갱신
    /// </summary>
    /// <param name="target">갱신할 UI 명칭</param>
    /// <param name="amount">갱신할 UI 수치</param>
    public void UpdateUIText(string target, float amount){
        if(target == null) return;

        // uiDictionary에서 target의 이름을 찾아서 해당 UI를 갱신
        foreach (var ui in _uiDictionary)
        {
            if(ui.Key == UIConstants.HP)
            {
                UpdateHPText(amount);
            }
        }
    }

    /// <summary>
    /// HP 텍스트 업데이트
    /// </summary>
    /// <param name="hp">갱신된 HP</param>
    public void UpdateHPText(float hp)
    {
        _txtHP.text = "HP : " + hp + " / 100";
    }

    #endregion
}
