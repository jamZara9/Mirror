using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
    public bool isSwitched = false;
    public Image image1;
    public Image image2;
    public Animator anim;

    /// <summary>
    /// 캐릭터 이미지 변경.
    /// </summary>
    /// <param name="sprite">바뀔 이미지</param>
    public void SwitchImage(Sprite sprite)
    {
        if (!isSwitched)
        {
            image2.sprite = sprite;
            anim.SetTrigger("SwitchFirst");
        }
        else
        {
            image1.sprite = sprite;
            anim.SetTrigger("SwitchSecond");
        }

        isSwitched = !isSwitched;
    }
    
    /// <summary>
    /// 초기 캐릭터 이미지 세팅.
    /// </summary>
    /// <param name="sprite">세팅할 이미지</param>
    public void SetImage(Sprite sprite)
    {
        if (!isSwitched)
        {
            image1.sprite = sprite;
        }
        else
        {
            image2.sprite = sprite;
        }
    }

    /// <summary>
    /// 현재 캐릭터의 이미지를 전달하는 함수.
    /// </summary>
    /// <returns>현재 이미지</returns>
    public Sprite GetImage()
    {
        if (!isSwitched)
        {
            return image1.sprite;
        }
        else
        {
            return image2.sprite;
        }
    }
}