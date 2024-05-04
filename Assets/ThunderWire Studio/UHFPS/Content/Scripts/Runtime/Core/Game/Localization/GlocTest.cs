using System.Collections;
using System.Collections.Generic;
using UHFPS.Runtime;
using UnityEngine;

public class GlocTest : MonoBehaviour
{
    public GString Text;

    private void Start()
    {
        Text.SubscribeGloc(text => Debug.Log(text));
    }
}
