using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_health_ratio : MonoBehaviour
{
    //체력바 UI
    public Image img_health_ratio;

    //체력 비율 0 ~ 0.75
    [SerializeField]
    private float ratio;

    // Start is called before the first frame update
    void Start()
    {
        ratio = 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        ratio -= 0.3f * Time.deltaTime;
        if (ratio < 0.01f)
            ratio = 0.75f;
        //테스트용 코드로 프로젝트 진행상황에 따라 이벤트 발생으로 처리 예정
        Mathf.Clamp(ratio, 0f, 0.75f);
        img_health_ratio.fillAmount = ratio;
    }
}
