using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundController : MonoBehaviour
{
    public bool isSwitched = false;
    public Image backGround1;
    public Image backGround2;
    public Animator anim;

    public void SwitchImage(Sprite sprite)
    {
        if (!isSwitched)
        {
            backGround2.sprite = sprite;
            anim.SetTrigger("SwitchFirst");
        }
        else
        {
            backGround1.sprite = sprite;
            anim.SetTrigger("SwitchSecond");
        }

        isSwitched = !isSwitched;
    }
    
    public void SetImage(Sprite sprite)
    {
        if (!isSwitched)
        {
            backGround1.sprite = sprite;
        }
        else
        {
            backGround2.sprite = sprite;
        }
    }
}