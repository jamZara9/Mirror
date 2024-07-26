using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    public static event Action<BaseItem, PlayerInventory> OnItemPickup;
    public static event Action<PlayerInventory> OnItemUse;


    public static void ItemPickup(BaseItem detectedItem, PlayerInventory playerInventory){
        OnItemPickup?.Invoke(detectedItem, playerInventory);
    }

    public static void ItemUse(PlayerInventory playerInventory){
        OnItemUse?.Invoke(playerInventory);
    }
    }
}
