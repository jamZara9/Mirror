using System;
using System.Collections;
using UnityEngine;

public class SpinCylinder : MonoBehaviour
{
    public CylinderSet cylinderSet;   // CylinderSet에서 지정된 값 받아오는 용도
    public bool ySpin;                // Y축 회전을 사용할 것인가

    [HideInInspector] public int myNum; // CylinderSet이 지정한 번호를 받음

    private float spinRotate;   // 한 번에 회전하는 각도
    private bool spin;          // 현재 회전 중인지 확인하는 용도
    private float speed;        // CylinderSet이 지정한 회전 속도를 받아오는 용도
    private float waitTime;     // 회전을 할 때 기다리는 시간
    
    private void Awake()
    {
        // 초기 값 세팅
        waitTime = 1f / cylinderSet.speed;
        speed = cylinderSet.speed;
        
        // CylinderSet이 설정한 면 수에 맞춰서 회전 각도를 지정함
        spinRotate = 360 / cylinderSet.cylinderSpinSet[myNum];
    }

    // CylinderSet에서 실행시킴
    public void PuzzleClick()
    {
        // 회전 중이 아닐 때 (연속 회전 방지)
        if (spin) return;
        
        spin = true;
        // CylinderSet에서 가지고 있는 이 객체의 현재 회전 값을 올림
        cylinderSet.puzzleNowAnswer[myNum]++;
        // 만약 기본 값이 현재값보다 같거나 작을 시 0으로 바꿈
        if (cylinderSet.cylinderSpinSet[myNum] <= cylinderSet.puzzleNowAnswer[myNum])
        {
            cylinderSet.puzzleNowAnswer[myNum] = 0;
        }
        // 회전 코루틴 실행
        StartCoroutine(Spin());
    }

    private void Update()
    {
        if(!spin) return;
        
        if (!ySpin) // y축 회전이 아닐 때
        {
            // 정해진 각도만큼 회전
            transform.Rotate(spinRotate * Time.deltaTime * speed, 0, 0);
        }
        else // y축 회전일 때
        {
            // 정해진 각도만큼 회전
            transform.Rotate(0, spinRotate * Time.deltaTime * speed, 0);
        }
    }

    private IEnumerator Spin()
    {
        // waitTime 이상의 시간이 지났을 경우 다음 코드를 진행
        yield return new WaitForSeconds(waitTime);

        if (!ySpin)     // y축 회전이 아닐 때
        {
            // 어긋난 각도 재조정
            transform.localRotation = Quaternion.Euler(spinRotate * cylinderSet.puzzleNowAnswer[myNum], 0, 0);
        }
        else        // y축 회전일 때
        {
            // 어긋난 각도 재조정
            transform.localRotation = Quaternion.Euler(0, spinRotate * cylinderSet.puzzleNowAnswer[myNum], 0);
        }
        // spin 초기화
        spin = false;
    }
}
