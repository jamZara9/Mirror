using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStoryEventOne : StoryEventBase
{
    // 스토리 이벤트 테스트용 코드.
    // StoryEventBase를 상속 후 로직 작성.
    
    // Awake 사용이 불가능하니 Awake 대신 Init 추상 함수 오버라이딩하여 사용.
    protected override void Init()
    {
        myIndex = 1; // 코드가 아니라 인스펙터 창에서도 세팅할 수 있음.
        Debug.Log("1번 이벤트 테스트 이닛");
    }
    
    public override void StoryEvent()
    {
        Debug.Log("테스트 이벤트 1번 실행");
    }
}
