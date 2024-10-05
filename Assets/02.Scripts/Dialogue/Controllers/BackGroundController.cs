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

    /// <summary>
    /// 배경 이미지 변경 (애니메이션 O)
    /// </summary>
    /// <param name="sprite">바뀔 이미지</param>
    public void SwitchImage(Sprite sprite)
    {
        if (!sprite)
        {
            return;
        }
        
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
    
    /// <summary>
    /// 배경 이미지 세팅 (애니메이션 X)
    /// </summary>
    /// <param name="sprite">바뀔 이미지</param>
    public void SetImage(Sprite sprite)
    {
        if (!sprite)
        {
            return;
        }
        
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