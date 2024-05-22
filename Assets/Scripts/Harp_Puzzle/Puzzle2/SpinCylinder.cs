using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpinCylinder : MonoBehaviour
{
    public CylinderSet cylinderSet;

    private Quaternion _quaternion;
    
    public bool spin;

    [HideInInspector] public int myNum;

    private float _spinRotate;
    private float _currentTime;
    private void Awake()
    {
        cylinderSet = GameObject.Find("Puzzle2").GetComponent<CylinderSet>();
    }

    private void Start()
    {
        
        _spinRotate = 360 / cylinderSet.cylinderSpinSet[myNum];
        Debug.Log(_spinRotate);
        StartCoroutine(Spin());
    }

    private void Update()
    {
        if (spin)
        {
            transform.Rotate(_spinRotate * Time.deltaTime * 4,0,0);
        }
        
    }

    public void Click()
    {
        if (!spin)
        {
            spin = true;
        }
    }

    IEnumerator Spin()
    {
        while (true)
        {
            yield return new WaitUntil((() => spin));
            yield return new WaitForSeconds(0.25f);
            cylinderSet.puzzleNowAnswer[myNum] += 1;
            if (cylinderSet.cylinderSpinSet[myNum] <= cylinderSet.puzzleNowAnswer[myNum])
            {
                cylinderSet.puzzleNowAnswer[myNum] = 0;
            }
            transform.rotation = Quaternion.Euler(_spinRotate * cylinderSet.puzzleNowAnswer[myNum],0,0);
            spin = false;
        }
        
    }
}
