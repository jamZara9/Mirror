using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStoryEventOne : StoryEventBase
{
    protected override void Init()
    {
        myIndex = 1;
        Debug.Log("1번 이벤트 테스트 이닛");
    }
    
    public override void StoryEvent()
    {
        Debug.Log("테스트 이벤트 1번 실행");
    }
}
