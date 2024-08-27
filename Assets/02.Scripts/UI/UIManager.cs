using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 상수를 관리하는 클래스
/// </summary>
public class UIConstants
{
    public const string UIGroup = "________UI__________";
    public const string HUDCanvas = "HUD_Canvas";
    public const string InventoryCanvas = "Inventory_Canvas";
    public const string QuickSlotCanvas = "QuickSlot_Canvas";
}

/// <summary>
/// UI를 관리하는 클래스
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas HUD_Canvas;
    [SerializeField]
    private Canvas Inventory_Canvas;
    [SerializeField]
    private Canvas QuickSlot_Canvas;

    public GameObject Text_ItemName;
    public GameObject Text_ItemDescription;

    [SerializeField] private GameObject _UIGroup;   // UI를 담고 있는 그룹
    // Test
    private Dictionary<string, Canvas> _canvasDictionary  = new();  // UI 캠버스 딕셔너리
    private void Awake()
    {
        Find_Canvas();
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
        ///////////////////////////////////////////////////////////////////////////////////////////////////// Test


        // HUD_Canvas = GameObject.Find(UIConstants.HUDCanvas)?.GetComponent<Canvas>();
        // Inventory_Canvas = GameObject.Find(UIConstants.InventoryCanvas)?.GetComponent<Canvas>();
        // QuickSlot_Canvas = GameObject.Find(UIConstants.QuickSlotCanvas)?.GetComponent<Canvas>();

        // HUD_Canvas.gameObject.SetActive(true);
        // Inventory_Canvas.gameObject.SetActive(true);
        // QuickSlot_Canvas.gameObject.SetActive(true);
    }

    private void Start()
    {
        // HUD_Canvas.gameObject.SetActive(true);
        // Inventory_Canvas.gameObject.SetActive(false);
        // QuickSlot_Canvas.gameObject.SetActive(false);

        foreach (var canvas in _canvasDictionary)
        {
            if(canvas.Key == UIConstants.HUDCanvas)
            {
                canvas.Value.gameObject.SetActive(true);
            }else{
                canvas.Value.gameObject.SetActive(false);
            }
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

}
