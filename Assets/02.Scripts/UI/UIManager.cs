using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas HUD_Canvas;
    [SerializeField]
    private Canvas Inventory_Canvas;
    [SerializeField]
    private Canvas QuickSlot_Canvas;

    private void Awake()
    {
        Find_Canvas();
    }
    public void Find_Canvas()
    {
        HUD_Canvas = GameObject.Find("HUD_Canvas")?.GetComponent<Canvas>();
        Inventory_Canvas = GameObject.Find("Inventory_Canvas")?.GetComponent<Canvas>();
        QuickSlot_Canvas = GameObject.Find("QuickSlot_Canvas")?.GetComponent<Canvas>();

        HUD_Canvas.gameObject.SetActive(true);
        Inventory_Canvas.gameObject.SetActive(true);
        QuickSlot_Canvas.gameObject.SetActive(true);
    }

    private void Start()
    {
        HUD_Canvas.gameObject.SetActive(true);
        Inventory_Canvas.gameObject.SetActive(false);
        QuickSlot_Canvas.gameObject.SetActive(false);
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





}
