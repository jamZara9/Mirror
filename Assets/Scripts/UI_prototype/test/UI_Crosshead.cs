using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Crosshead : MonoBehaviour
{
    //크로스헤드 이미지 저장 컨테이너
    public List<Sprite> CrossHead_Sprites = new List<Sprite>(2);

    private Image img_crossHead;


    // Start is called before the first frame update
    void Start()
    {
        img_crossHead = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            img_crossHead.sprite = CrossHead_Sprites[1];
        }
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            img_crossHead.sprite = CrossHead_Sprites[0];
        }

    }
}
