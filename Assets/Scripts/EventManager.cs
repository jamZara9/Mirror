using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    public static event Action<GameObject, PlayerInventory> OnItemPickup;
    public static event Action<GameObject, PlayerInventory> OnItemUse;

    public static void ItemPickup(GameObject detectedItem, PlayerInventory playerInventory){
        OnItemPickup?.Invoke(detectedItem, playerInventory);
    }

    public static void ItemUse(GameObject selectedItem, PlayerInventory playerInventory){
        OnItemUse?.Invoke(selectedItem, playerInventory);
    }
}
