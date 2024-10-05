using System;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

public class Scratch : RaycastCheck, IInteractionable
{
    [Header("코드 블록의 이름 마지막 글자는 모두 1~4 사이의 숫자로 통일")]
    public GameObject[] codeBlock;      // 코드 블록을 담을 리스트
    public Camera myCam;                // Raycast 및 화면 전환할 카메라
    public GameObject inputBlock;       // 코드 블록이 들어갈 위치
    public GameObject playButton;       // 실행 버튼

    public bool test;                    // 상호작용 테스트 용 변수   *임시*

    private Vector3[] codeBlockSetPos;   // 코드 블록들의 시작 위치
    private bool interaction;            // 상호 작용 확인
    private bool drag;                   // 드래그 중인지 확인할 bool 값
    private int nowDragButton;           // 현재 드래그 중인 버튼 확인 용도

    private void Start()
    {
        // 코드 블록 기본 위치 세팅
        codeBlockSetPos = new Vector3[codeBlock.Length];
        for(int i = 0; i < codeBlock.Length; i++)
        {
            codeBlockSetPos[i] = codeBlock[i].transform.localPosition;
        }
    }

    private void Update()
    {
        if (test)       // 상호작용 테스트 용  *임시*
        {
            Interaction();
            test = false;
        }

        if (Input.GetKeyUp(KeyCode.Escape))      // 상호작용 테스트 용  *임시*
        {
            EndInteraction();
        }
        
        if (!interaction) return;   // 상호 작용 중일때만 사용할 수 있도록 함
        
        Drag(); // 드래그 감지
        BlockMove(); // 버튼을 움직이는 용도
    }
    private void Drag()
    {
        // 좌클릭을 눌렀을 때
        if(Input.GetMouseButtonDown(0)){
            // 실행 버튼을 클릭했을 때
            if (RayHitCheck(Input.mousePosition, myCam, playButton.transform))
            {
                // 만약 코드 블록이 들어가 있을 때
                if (inputBlock.transform.GetChild(0) != null)
                {
                    // 코드 블록에 따른 이벤트를 실행한다.
                    PlayCode(inputBlock.transform.GetChild(0).gameObject);
                }
                // 아니면
                else
                {
                    // 에러를 띄운다
                    Debug.Log("Error");
                }
            }
            
            // 버튼의 수만큼 반복
            for (var i = 0; i < codeBlock.Length; i++)
            {
                // Ray가 물체와 충돌하지 않았거나,  현재 비교 중인 객체와 충돌한 객체가 같은 경우
                if (!RayHitCheck(Input.mousePosition, myCam, codeBlock[i].transform)) continue;
                codeBlock[i].transform.SetParent(gameObject.transform);
                // 현재 드래그 중인 버튼을 i번째 버튼으로 지정
                nowDragButton = i;
                // 드래그 중으로 변환
                drag = true;
            }
            
        }
        
        // 좌클릭이 끝났을 때
        if(Input.GetMouseButtonUp(0)){
            // 드래그 중지
            drag = false;
            // 코드 블록과 Input Block의 거리를 잰다
            var distance = Vector3.Distance(codeBlock[nowDragButton].transform.position, inputBlock.transform.position);

            // 일정 범위 내에 코드 블록이 떨어졌다면
            if (distance < 0.75f)
            {
                // 이미 Input Block안에 블록이 들어가 있을 경우
                if (inputBlock.transform.childCount > 0)
                {
                    // Input Block 안의 코드 블록을 조금 이동시킨 뒤
                    inputBlock.transform.GetChild(0).transform.localPosition = new Vector3(-0.1f, 0, 0.15f);
                    // 코드 블록의 부모를 바꿔준다
                    inputBlock.transform.GetChild(0).SetParent(gameObject.transform);
                }
                // 드랍한 코드 블록의 부모를 Input Block로 변경해준다.
                codeBlock[nowDragButton].transform.SetParent(inputBlock.transform);
                // 코드 블록의 위치를 Input Block의 위치로 변경해준다.
                codeBlock[nowDragButton].transform.localPosition = new Vector3(0, 0, 0);
            }
            // 드래그 종료 시에 코드블록이 카메라를 벗어났을 경우
            if (!CheckInCam(codeBlock[nowDragButton]))
            {
                // 코드 블록의 위치를 원래 위치로 되돌려준다.
                codeBlock[nowDragButton].transform.localPosition = codeBlockSetPos[nowDragButton];
            }
        }
    }

    private void BlockMove()
    {
        // 현재 드래그 중인 블럭의 localPosition값을 받아옴
        var buttonPos = codeBlock[nowDragButton].transform.localPosition;
        
        // 드래그 중일 때
        if(drag)
        {
            // 마우스의 위치 값을 저장
            Vector3 position = new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, myCam.WorldToScreenPoint(transform.position).z);
            
            // 카메라 기준으로 position 값을 저장
            Vector3 worldPosition = myCam.ScreenToWorldPoint(position);

            // 블럭이 마우스를 따라가도록 함
            codeBlock[nowDragButton].transform.localPosition = new Vector3(worldPosition.x, buttonPos.y, worldPosition.z);
        }
    }

    private bool CheckInCam(GameObject dragBlock)
    {
        // dragBlock의 카메라 상 위치 값을 받아온다.
        Vector3 screenPoint = myCam.WorldToViewportPoint(dragBlock.transform.position);
        // dragBlock의 위치가 화면 안인지 체크해 bool 값으로 받아온다.
        bool inScreen = screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0; 
        // dragBlock이 화면 안에 존재하는지 아닌지 알려준다
        return inScreen;
    }
    
    // 상호작용
    public void Interaction()
    {
        // 상호작용 시작
        interaction = true;
        // 카메라를 켠다
        myCam.gameObject.SetActive(true);
    }
    
    // 상호작용 종료
    private void EndInteraction()
    {
        // 카메라를 끈다
        myCam.gameObject.SetActive(false);
        // 상호작용 종료
        interaction = false;
    }

    // 들어가 있는 코드 블록에 따라 이벤트를 실행한다.
    private void PlayCode(GameObject getCodeBlock)
    {
        // Debug.Log($"Num = {getCodeBlock.name.Substring(getCodeBlock.name.Length - 1, 1)}");

        // getCodeBlock의 이름의 가장 뒷 부분의 숫자를 받아온다
        switch (Convert.ToInt32(getCodeBlock.name.Substring(getCodeBlock.name.Length - 1, 1)))
        {
            // 숫자에 따라 각 이벤트가 실행된다 (추후 변경 예정)
            case 1:
                Debug.Log("괴물이 들어온다");
                break;
            case 2:
                Debug.Log("시청각실");
                break;
            case 3:
                Debug.Log("안돼, 넌 할 수 있어");
                break;
            case 4:
                Debug.Log("받아들여");
                break;
        }
        // 상호작용을 종료한다
        EndInteraction();
    }
}
