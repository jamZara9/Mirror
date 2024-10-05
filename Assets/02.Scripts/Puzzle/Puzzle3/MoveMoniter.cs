using System.Collections;
using UnityEngine;

public class MoveMoniter : MonoBehaviour, IInteractionable
{
    public float spinRotate = 30f;  // 한번에 회전하는 각
    public float movePos = 1f;      // 한번에 움직이는 거리

    public int nowAngle;            // (-1 : 왼쪽, 0 : 중간, 1 : 오른쪽) 과 같은 방식으로 현재 위치 및 방향을 확인하는 용도
    private bool move;              // 움직이는 중인지 확인
    private bool leftMove;          // true일 때 왼쪽으로 이동, false일 때 오른쪽으로 이동

    private Vector3 startRot;       // 시작 각도
    private Quaternion leftRot;     // 왼쪽으로 이동했을 때의 각도
    private Quaternion rightRot;    // 오른쪽으로 이동했을 때의 각도
    private Vector3 leftDir;        // 왼쪽으로 이동할 위치
    private Vector3 rightDir;       // 오른쪽으로 이동할 위치
    private Vector3 startPos;       // 시작 위치
    
    public enum MoveState           // 오브젝트를 회전 or 이동 용도로 사용할 것인지 선택
    {                               
        Move,
        Spin
    }
    public MoveState moveState;     // 오브젝트를 회전 or 이동 용도로 사용할 것인지 선택
    
    
    private void Start()
    {
        // 각도 변수들 초기화
        startRot = transform.rotation.eulerAngles;
        leftRot = Quaternion.Euler(0, startRot.y - spinRotate, 0);
        rightRot = Quaternion.Euler(0, startRot.y + spinRotate, 0);
        
        // 위치 변수들 초기화
        startPos = transform.localPosition;
        leftDir = new Vector3(transform.localPosition.x + movePos, 0, 0);
        rightDir = new Vector3(transform.localPosition.x - movePos, 0, 0);
    }

    void Update()
    {
        // move가 true일 때만 진행
        if (!move) return;
        switch (moveState) // moveState의 값에 따라
        {
            //회전
            case MoveState.Spin:
                Spin();
                break;
            // 좌우 이동
            case MoveState.Move:
                Move();
                break;
        }
    }

    // Update에서 실행
    private void Spin()
    {
        if (nowAngle == 0) // nowAngle이 0일 때 
        {
            // 현재 각도를 startRot로 변경
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(startRot), 1);
        }
        else // nowAngle이 0이 아닐 때 
        {
            // leftMove가 true일 땐 각도를 leftRot로 변경
            // leftMove가 false일 땐 각도를 rightRot로 변경
            transform.rotation = Quaternion.RotateTowards(transform.rotation, leftMove ? leftRot : rightRot, 1);
        }
    }
    // Update에서 실행
    private void Move()
    {
        if (nowAngle == 0) // nowAngle이 0일 때 
        {
            // 현재 각도를 startPos로 변경
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPos, 0.25f);
        }
        else // nowAngle이 0이 아닐 때 
        {
            // leftMove가 true일 땐 localPosition을 leftDir로 변경
            // leftMove가 false일 땐 localPosition을 rightDir로 변경
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, leftMove ? leftDir : rightDir, 0.25f);
        }
    }

    // 플레이어와의 상호작용으로 실행
    public void Interaction()
    {
        if (move) return;   // move가 false일 때 진행
        if (leftMove)   // leftMove가 true일 때
        {
            // nowAngle을 왼쪽으로 1칸 이동
            nowAngle -= 1;
        }
        else    // leftMove가 false일 때
        {
            // nowAngle을 오른쪽으로 1칸 이동
            nowAngle += 1;
        }
        move = true;    // 이동 중으로 변경
        StartCoroutine(WaitTime()); //WaitTime 코루틴 실행
    }

    // PuzzleClick이 실행
    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(0.5f);  //실행 후 0.5초 대기
        move = false;   // 움직임 중지
        if (nowAngle != 0)  // nowAngle이 0이 아닐 시
        {
            // 왼쪽 <-> 오른쪽으로 교체
            leftMove = !leftMove;
        }
        switch (moveState)  // moveState에 따라 실행
        {
            case MoveState.Spin:    // Spin일 때
                WaitTimeSpin();
                break;
            case MoveState.Move:    //Move일 때
                WaitTimeMove();
                break;
        }
    }
    // WaitTime에서 실행
    private void WaitTimeSpin()
    {
        // nowAngle에 따라 어긋난 각도 재조정
        switch (nowAngle)
        {
            case -1:    // leftRot로 재조정
                transform.rotation = leftRot;
                break;
            case 0:     // startRot로 재조정
                transform.rotation = Quaternion.Euler(startRot);
                break;
            case 1:     // RightRot로 재조정
                transform.rotation = rightRot;
                break;
        }
    }
    // WaitTime에서 실행
    private void WaitTimeMove()
    {
        // nowAngle에 따라 어긋난 위치 재조정
        switch (nowAngle)
        {
            case -1:    //  leftDir로 재조정
                transform.localPosition = leftDir;
                break;
            case 0:     // startPos로 재조정
                transform.localPosition = startPos;
                break;
            case 1:     // rightDir로 재조정
                transform.localPosition = rightDir;
                break;
        }
    }
}
