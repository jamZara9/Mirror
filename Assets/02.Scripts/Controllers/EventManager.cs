using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    public static event Action<IInventoryItem, PlayerInventory> OnItemPickup;
    public static event Action<PlayerInventory> OnItemUse;
    public static event Action<GameObject> ShowInventory;


    public static void ItemPickup(IInventoryItem detectedItem, PlayerInventory playerInventory){
        OnItemPickup?.Invoke(detectedItem, playerInventory);
    }

    public static void ItemUse(PlayerInventory playerInventory){
        OnItemUse?.Invoke(playerInventory);
    }

    public static void ShowInventoryUI(GameObject inventoryUI){
        ShowInventory?.Invoke(inventoryUI);
    }
}
