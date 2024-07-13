using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MoveMoniter : MonoBehaviour
{
    public float spinRotate = 30f;
    public float movePos = 1f;

    public int nowAngle; // -1 : 왼쪽, 0 : 중간, 1 : 오른쪽
    private bool move;
    private bool leftMove;

    private Vector3 startRot;
    private Quaternion leftRot;
    private Quaternion rightRot;
    
    private Vector3 startPos;
    private Vector3 leftPos;
    private Vector3 rightPos;

    public GameObject[] setTransform;
    
    public enum MoveState
    {
        Move,
        Spin
    }
    public MoveState movestate;
    private void Start()
    {
        startRot = transform.rotation.eulerAngles;
        leftRot = Quaternion.Euler(0, startRot.y - spinRotate, 0);
        rightRot = Quaternion.Euler(0, startRot.y + spinRotate, 0);

        
        startPos = transform.position;
        var leftDir = setTransform[0].transform.position;
        var rightDir = setTransform[1].transform.position;
        Debug.Log(leftDir+"   "+ rightDir);

        leftDir = new Vector3(Mathf.Approximately(leftDir.x, startPos.x) ? 
                startPos.x : 
                leftDir.x > rightDir.x ? leftDir.x + movePos : leftDir.x - movePos,
            Mathf.Approximately(leftDir.y, startPos.y) ? 
                startPos.y : 
                leftDir.y > rightDir.y ? leftDir.y + movePos : leftDir.y - movePos,
            Mathf.Approximately(leftDir.z, startPos.z) ? 
                startPos.z :
                leftDir.z > rightDir.z ? leftDir.z + movePos : leftDir.z - movePos);
        rightDir = new Vector3(Mathf.Approximately(rightDir.x, startPos.x) ?
                startPos.x : 
                rightDir.x > leftDir.x ? rightDir.x + movePos : rightDir.x - movePos,
            Mathf.Approximately(rightDir.y, startPos.y) ? 
                startPos.y : 
                rightDir.y > leftDir.y ? rightDir.y + movePos : rightDir.y - movePos,
            Mathf.Approximately(rightDir.z, startPos.z) ? 
                startPos.z : 
                rightDir.z > leftDir.z ? rightDir.z + movePos : rightDir.z - movePos);
        
        Debug.Log(leftDir+"   "+ rightDir);

        leftPos = leftDir;
        rightPos = rightDir;
        
        Debug.Log(leftPos+"   "+ rightPos);
    }

    void Update()
    {
        if (!move) return;
        switch (movestate)
        {
            //회전
            case MoveState.Spin when nowAngle == 0:
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation,
                        Quaternion.Euler(startRot), 1);
                break;
            case MoveState.Spin:
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation,
                        (leftMove ? leftRot : rightRot), 1);
                break;
            
            // 좌우 이동
            case MoveState.Move when nowAngle == 0:
                transform.position =
                    Vector3.MoveTowards(transform.position,
                        startPos, 0.25f);
                break;
            case MoveState.Move:
                transform.position =
                    Vector3.MoveTowards(transform.position,
                        (leftMove ? leftPos : rightPos), 0.25f);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PuzzleClick();
        }
    }

    private void PuzzleClick()
    {
        if (move) return;
        if (leftMove)
        {
            nowAngle -= 1;
            move = true;
            StartCoroutine(WaitTime());
        }
        else
        {
            nowAngle += 1;
            move = true;
            StartCoroutine(WaitTime());
        }
        
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(0.5f);
        move = false;
        if (nowAngle != 0)
        {
            leftMove = !leftMove;
        }
        switch (nowAngle)
        {
            // 회전
            case -1 when movestate == MoveState.Spin:
                transform.rotation = leftRot;
                break;
            case 0 when movestate == MoveState.Spin:
                transform.rotation = Quaternion.Euler(startRot);
                break;
            case 1 when movestate == MoveState.Spin:
                transform.rotation = rightRot;
                break;
            // 좌우 이동
            case -1 when movestate == MoveState.Move:
                transform.position = leftPos;
                break;
            case 0 when movestate == MoveState.Move:
                transform.position = startPos;
                break;
            case 1 when movestate == MoveState.Move:
                transform.position = rightPos;
                break;
        }
    }
}
