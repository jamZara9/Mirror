using UnityEngine;

public class AudioMixerPuzzle : MonoBehaviour
{
    public float maxMove;               // 슬라이더가 상하로 이동할 수 있는 최대 거리
    public float buttonMoveSpeed = 1;   // 버튼이 이동하는 속도
    public GameObject[] button;         // 버튼들을 담을 리스트
    public Camera myCam;                // Raycast용 카메라
    public bool open;
    
    private bool drag;                   // 드래그 중인지 확인할 bool 값
    private int nowDragButton;           // 현재 드래그 중인 버튼 확인 용도
    void Update()
    {
        Drag();             // 드래그 감지
        ButtonMove();       // 버튼을 움직이는 용도
        CheckClear();       // 클리어 확인 용도
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

            // 버튼의 수만큼 반복
            for (var i = 0; i < button.Length; i++)
            {
                // Ray가 물체와 충돌하였고,  현재 비교 중인 객체와 충돌한 객체가 같은 경우
                if (weHitSomething && raycastHit.transform == button[i].transform)
                {
                    // 현재 드래그 중인 버튼을 i번째 버튼으로 지정
                    nowDragButton = i;
                    // 드래그 중으로 변환
                    drag = true;
                }
            }
            
        }
        
        // 좌클릭이 끝났을 때
        if(Input.GetMouseButtonUp(0)){
            // 드래그 중지
            drag = false;
        }
    }

    private void ButtonMove()
    {
        // 현재 드래그 중인 버튼의 localPosition값을 받아옴
        var buttonPos = button[nowDragButton].transform.localPosition;
        
        // 드래그 중일 때
        if(drag)
        {
            // 마우스의 위치 값을 저장
            Vector3 position = new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, myCam.WorldToScreenPoint(transform.position).z);
            
            // 카메라 기준으로 position 값을 저장
            Vector3 worldPosition = myCam.ScreenToWorldPoint(position);
            
            // 드래그 중인 버튼의 위치가 최대로 이동할 수 있는 상하 값보다 작을 경우
            if(buttonPos.z >= -maxMove && buttonPos.z <= maxMove)
            {
                // 버튼의 localPosition을 기준으로 z축을 마우스의 이동 값에 따라 이동시킴
                button[nowDragButton].transform.localPosition = new Vector3(buttonPos.x, buttonPos.y, worldPosition.z * buttonMoveSpeed);
            }
        }
        
        // 버튼이 상하 최대 값을 벗어났을 경우
        if(buttonPos.z > maxMove)
        {
            // 버튼이 이동할 수 있는 최대 값의 위치로 이동시킴
            button[nowDragButton].transform.localPosition = new Vector3(buttonPos.x, buttonPos.y, maxMove);
            // 드래그 종료 (상하로 계속 이동하는 것 방지)
            drag = false;
        }
        else if(buttonPos.z < -maxMove)
        {
            // 버튼이 이동할 수 있는 최대 값의 위치로 이동시킴
            button[nowDragButton].transform.localPosition = new Vector3(buttonPos.x, 0, -maxMove);
            // 드래그 종료 (상하로 계속 이동하는 것 방지)
            drag = false;
        }
    }

    private void CheckClear()
    {
        var check = 0;          // 통과 조건을 만족시킨 버튼 개수 체크 용
        
        // 각 버튼마다 값을 확인
        foreach (var t in button)
        {
            // 현재 체크 중인 버튼의 위치가 최상단에 위치할 경우
            if (t.transform.localPosition.z >= maxMove)
            {
                // 통과한 버튼의 개수를 1개 추가
                check++;
            }
            else
            {
                // 아닐 경우에 반복문 종료
                break;
            }
        }

        // 모든 버튼이 통과 조건을 만족했을 시
        if (check == button.Length)
        {
            // 클리어로 변경함
            open = true;
            Debug.Log("Clear!");
        }
    }
}
