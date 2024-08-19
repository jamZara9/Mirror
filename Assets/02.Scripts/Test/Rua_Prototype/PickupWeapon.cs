using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : MonoBehaviour, IInteractionObject
{
    public GameObject weaponObject;
    public object Interaction()
    {
        return weaponObject;
    }

}
