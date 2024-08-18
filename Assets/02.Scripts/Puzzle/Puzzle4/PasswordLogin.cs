using TMPro;
using UnityEngine;

public class PasswordLogin : MonoBehaviour
{
    // 
    public TMP_InputField nowText;      // 현재 InputField에 적혀있는 답

    public string password;             // 정답

    // 작성 후 Enter키를 눌렀을 때 실행됨
    public void Enter()
    {
        // 현재 답과 정답이 일치할 경우 클리어
        if (nowText.text == password)
        {
            Debug.Log("clear");
        }
        // 아니면 필드에 작성된 텍스트를 지운다
        else
        {
            nowText.text = "";
            Debug.Log("Error");
        }
    }
}
