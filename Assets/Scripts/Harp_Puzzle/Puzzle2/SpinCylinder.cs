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

    private float _speed;
    private float _waitTime;
    public enum SpinPos
    {
        X,
        Y,
        Z
    }

    public SpinPos spinPos;

    private void Awake()
    {
        _waitTime = 1f / cylinderSet.speed;
        _speed = cylinderSet.speed;
        Debug.Log(transform.name + "  " + _waitTime);
        _spinRotate = 360 / cylinderSet.cylinderSpinSet[myNum];
        StartCoroutine(Spin());
    }

    private void Update()
    {
        if (spin)
        {
            switch (spinPos)
            {
                case SpinPos.X:
                    transform.Rotate(_spinRotate * Time.deltaTime * _speed,0,0);
                    break;
                case SpinPos.Y:
                    transform.Rotate(0,_spinRotate * Time.deltaTime * _speed,0);
                    break;
                case SpinPos.Z:
                    transform.Rotate(0,0,_spinRotate * Time.deltaTime * _speed);
                    break;
            }
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
            yield return new WaitForSeconds(_waitTime);
            cylinderSet.puzzleNowAnswer[myNum]++;
            if (cylinderSet.cylinderSpinSet[myNum] <= cylinderSet.puzzleNowAnswer[myNum])
            {
                cylinderSet.puzzleNowAnswer[myNum] = 0;
            }
            switch (spinPos)
            {
                case SpinPos.X:
                    transform.rotation = Quaternion.Euler(_spinRotate * cylinderSet.puzzleNowAnswer[myNum],0,0);
                    break;
                case SpinPos.Y:
                    transform.rotation = Quaternion.Euler(0,_spinRotate * cylinderSet.puzzleNowAnswer[myNum],0);
                    break;
                case SpinPos.Z:
                    transform.rotation = Quaternion.Euler(0,0,_spinRotate * cylinderSet.puzzleNowAnswer[myNum]);
                    break;
            }
            spin = false;
        }
        
    }
}
