using System;
using TMPro;
using UnityEngine;

public class PasswordLogin : MonoBehaviour, IInteractionable
{
    public bool test;                  // 상호작용 테스트 용 변수 *임시*
    
    public TMP_InputField nowText;      // 현재 InputField에 적혀있는 답

    public string password;             // 정답

    public GameObject myCam;                // 카메라 전환 용 카메라

    private bool interaction;           // 상호작용 변수

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
            nowText.gameObject.SetActive(false);
            // 상호작용 종료
            interaction = false;
        }
    }

    // 작성 후 Enter키를 눌렀을 때 실행됨
    public void Enter()
    {
        if (!interaction) return;   // 상호 작용 중일때만 사용할 수 있도록 함
        // 현재 답과 정답이 일치할 경우 클리어
        if (nowText.text == password)
        {
            Debug.Log("clear");
            // 카메라를 끈다
            myCam.gameObject.SetActive(false);
            nowText.gameObject.SetActive(false);
            // 상호작용 종료
            interaction = false;
        }
        // 아니면 필드에 작성된 텍스트를 지운다
        else
        {
            nowText.text = "";
            Debug.Log("Error");
        }
    }

    public void Interaction()
    {
        // 버츄얼 카메라를 켠다
        myCam.SetActive(true);
        nowText.gameObject.SetActive(true);
        // 퍼즐을 풀 수 있도록 한다
        interaction = true;
    }
}
