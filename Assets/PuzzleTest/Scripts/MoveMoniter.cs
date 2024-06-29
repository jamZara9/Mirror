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

    private int nowAngle; // -1 : 왼쪽, 0 : 중간, 1 : 오른쪽
    private bool move;
    private bool leftMove;

    private Quaternion startRot;
    private Quaternion leftRot;
    private Quaternion rightRot;
    
    private Vector3 startPos;
    private Vector3 leftPos;
    private Vector3 rightPos;
    
    public enum MoveState
    {
        Move,
        Spin
    }
    public MoveState movestate;
    private void Start()
    {
        startRot = transform.rotation;
        leftRot = Quaternion.Euler(0, startRot.y - spinRotate, 0);
        rightRot = Quaternion.Euler(0, startRot.y + spinRotate, 0);
        
        startPos = transform.position;
        leftPos = startPos + Vector3.left * movePos;
        rightPos = startPos + Vector3.right * movePos;
    }

    void Update()
    {
        switch (movestate)
        {
            //회전
            case MoveState.Spin when nowAngle == 0:
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation,
                        startRot, 1);
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
                        startPos, 0.5f);
                break;
            case MoveState.Move:
                transform.position =
                    Vector3.MoveTowards(transform.position,
                        (leftMove ? leftPos : rightPos), 0.5f);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log(other + " 감지");
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
        switch (nowAngle)
        {
            // 회전
            case -1 when movestate == MoveState.Spin:
                transform.rotation = leftRot;
                break;
            case 0 when movestate == MoveState.Spin:
                transform.rotation = startRot;
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
        if (nowAngle != 0)
        {
            leftMove = !leftMove;
        }
        move = false;
    }
}
