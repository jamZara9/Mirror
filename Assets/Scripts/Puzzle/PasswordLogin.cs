using TMPro;
using UnityEngine;

public class PasswordLogin : MonoBehaviour
{
    public TMP_InputField nowText;

    public string password;

    public void Enter()
    {
        if (nowText.text == password)
        {
            Debug.Log("clear");
        }
        else
        {
            nowText.text = "";
            Debug.Log("Error");
        }
    }
}
