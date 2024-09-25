using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CylinderSet : RaycastCheck, IInteractionable
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

    public bool test;                       // 상호작용 테스트 용 변수   *임시*
    

    private bool interaction;
    
    private bool open;                      // 답이 맞을 경우 true가 됨

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
        if (test)       // 상호작용 테스트 용  *임시*
        {
            Interaction();
            test = false;
        }

        if (Input.GetKeyUp(KeyCode.Escape))      // 상호작용 테스트 용  *임시*
        {
            // 카메라를 끈다
            myCam.gameObject.SetActive(false);
            // 상호작용 종료
            interaction = false;
        }
        
        if (!interaction) return;   // 상호 작용 중일때만 사용할 수 있도록 함
        
        // puzzleNowAnswer와 puzzleAnswer의 리스트 값이 같을 경우
        if (puzzleAnswer.SequenceEqual(puzzleNowAnswer))
        {
            // 클리어 판정
            open = true;
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
        // spinCylinder 안의 객체 별로 비교
        foreach (var checkCylinder in spinCylinder)
        {
            // Ray가 물체와 충돌하였고,  현재 비교 중인 객체와 충돌한 객체가 같은 경우
            if (RayHitCheck(Input.mousePosition, myCam, checkCylinder.transform))
            {
                // checkCylinder 객체의 PuzzleClick을 실행
                checkCylinder.GetComponent<SpinCylinder>().PuzzleClick();
            }
        }
    }

    // 플레이어가 상호작용을 진행했을 경우
    public void Interaction()
    {
        // 카메라를 켠다
        myCam.gameObject.SetActive(true);
        // 퍼즐을 풀 수 있도록 한다
        interaction = true;
    }
}
