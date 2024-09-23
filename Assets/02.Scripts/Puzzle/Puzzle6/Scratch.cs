using System;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

public class Scratch : MonoBehaviour, IInteractionable
{
    public GameObject[] codeBlock;         // 버튼들을 담을 리스트
    public Camera myCam;                // Raycast 및 화면 전환할 카메라
    public GameObject inputBlock;
    public GameObject playButton;
    public bool open;

    public bool test;                    // 상호작용 테스트 용 변수   *임시*

    private Vector3[] codeBlockSetPos;
    private bool interaction;            // 상호 작용 확인
    private bool drag;                   // 드래그 중인지 확인할 bool 값
    private int nowDragButton;           // 현재 드래그 중인 버튼 확인 용도

    private void Start()
    {
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
            // 카메라를 끈다
            myCam.gameObject.SetActive(false);
            // 상호작용 종료
            interaction = false;
        }
        
        if (!interaction) return;   // 상호 작용 중일때만 사용할 수 있도록 함
        
        Drag(); // 드래그 감지
        BlockMove(); // 버튼을 움직이는 용도
    }
    private void Drag()
    {
        // 좌클릭을 눌렀을 때
        if(Input.GetMouseButtonDown(0)){
            // 마우스의 위치 값을 가져옴
            Vector3 mousePosition = Input.mousePosition;
        
            // 카메라의 마우스 위치에서 Ray를 생성
            Ray myRay = myCam.ScreenPointToRay(mousePosition);
        
            // Ray의 충돌 확인 용도
            RaycastHit raycastHit;
        
            // Ray가 물체와 충돌했을 시 true, 아니면 false
            bool weHitSomething = Physics.Raycast(myRay, out raycastHit);

            if (weHitSomething && raycastHit.transform == playButton.transform)
            {
                if (inputBlock.transform.GetChild(0) != null)
                {
                    PlayCode(inputBlock.transform.GetChild(0).gameObject);
                }
                else
                {
                    Debug.Log("Error");
                }
            }
            // 버튼의 수만큼 반복
            for (var i = 0; i < codeBlock.Length; i++)
            {
                // Ray가 물체와 충돌하지 않았거나,  현재 비교 중인 객체와 충돌한 객체가 같은 경우
                if (!weHitSomething || raycastHit.transform != codeBlock[i].transform) continue;
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
            var distance = Vector3.Distance(codeBlock[nowDragButton].transform.position, inputBlock.transform.position);

            if (distance < 0.75f)
            {
                if (inputBlock.transform.childCount > 0)
                {
                    inputBlock.transform.GetChild(0).transform.localPosition = new Vector3(-0.1f, 0, 0.15f);
                    inputBlock.transform.GetChild(0).SetParent(gameObject.transform);
                }
                codeBlock[nowDragButton].transform.SetParent(inputBlock.transform);
                codeBlock[nowDragButton].transform.localPosition = new Vector3(0, 0, 0);
                
                // Debug.Log(inputBlock.transform.GetChild(0).transform.name);
            }

            if (!CheckInCam(codeBlock[nowDragButton]))
            {
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

    private bool CheckInCam(GameObject dragButton)
    {
        Vector3 screenPoint = myCam.WorldToViewportPoint(dragButton.transform.position);
        bool inScreen = screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0; 
        return inScreen;
    }
    
    public void Interaction()
    {
        interaction = true;
        myCam.gameObject.SetActive(true);
    }

    private void PlayCode(GameObject getCodeBlock)
    {
        // Debug.Log($"Num = {getCodeBlock.name.Substring(getCodeBlock.name.Length - 1, 1)}");

        switch (Convert.ToInt32(getCodeBlock.name.Substring(getCodeBlock.name.Length - 1, 1)))
        {
            // 추후 변경
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
    }
    
    
}
