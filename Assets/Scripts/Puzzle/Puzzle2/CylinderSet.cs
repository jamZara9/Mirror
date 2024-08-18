using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CylinderSet : MonoBehaviour
{
    [Tooltip("실린더 순서대로 넣기")]
    public SpinCylinder[] spinCylinder;     // 회전할 객체를 넣을 리스트
    
    [Tooltip("최대 회전 횟수")]
    public int[] cylinderSpinSet;           // 각 spinCylinder마다 회전할 수 있는 최대 회전 수
    
    [Tooltip("각 실린더 별 답")]
    public int[] puzzleAnswer;              // 각 실린더 별로 맞춰져야하는 답
    
    [Tooltip("각 실린더 별 현재 답")]
    // [HideInInspector]
    public int[] puzzleNowAnswer;           // 각 실린더의 현재 답

    private bool _open;                     // 답이 맞을 경우 true가 됨

    public float speed;                     // 실린더들의 회전 속도
    
    public Camera myCam;                    // Raycast에 사용되는 카메라
    
    void Awake()
    {
        // puzzleNowAnswer의 리스트 수를 초기화
        puzzleNowAnswer = new int[puzzleAnswer.Length];
        
        // 각 실린더별 초기 세팅을 위한 반복문
        for (int i = 0; i < spinCylinder.Length; i++)
        {
            // 컴포넌트 받아오기
            spinCylinder[i] = spinCylinder[i].GetComponent<SpinCylinder>();
            // SpinCylinder의 순서를 i로 지정
            spinCylinder[i].myNum = i;
            // puzzleNowAnswer의 현재 답을 0으로 세팅
            puzzleNowAnswer[i] = 0;
        }
    }

    void Update()
    {
        // puzzleNowAnswer와 puzzleAnswer의 리스트 값이 같을 경우
        if (puzzleAnswer.SequenceEqual(puzzleNowAnswer))
        {
            // 클리어 판정
            _open = true;
            Debug.Log("Clear!");
        }
        // 좌클릭을 했을 때
        if (Input.GetMouseButtonDown(0))
        {
            CylinderClick();
        }
    }

    private void CylinderClick()
    {
        // 마우스의 위치 값을 가져옴
        Vector3 mousePosition = Input.mousePosition;
        
        // 카메라의 마우스 위치에서 Ray를 생성
        Ray myRay = myCam.ScreenPointToRay(mousePosition);
        
        // Ray의 충돌 확인 용도
        RaycastHit raycastHit;
        
        // Ray가 물체와 충돌했을 시 true, 아니면 false
        bool weHitSomething = Physics.Raycast(myRay, out raycastHit);

        // spinCylinder 안의 객체 별로 비교
        foreach (var checkCylinder in spinCylinder)
        {
            // Ray가 물체와 충돌하였고,  현재 비교 중인 객체와 충돌한 객체가 같은 경우
            if (weHitSomething && raycastHit.transform == checkCylinder.transform)
            {
                // checkCylinder 객체의 PuzzleClick을 실행
                checkCylinder.GetComponent<SpinCylinder>().PuzzleClick();
            }
        }
    }
}
