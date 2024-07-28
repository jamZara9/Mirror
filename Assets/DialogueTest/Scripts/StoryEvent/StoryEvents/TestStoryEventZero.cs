using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStoryEventZero : StoryEventBase
{
    protected override void Init()
    {
        myIndex = 0;
        Debug.Log("0번 이벤트 테스트 이닛");
    }

    public override void StoryEvent()
    {
        Debug.Log("테스트 이벤트 0번 실행");
        StoryEventManager.Instance.SuccessEvent();
    }
}
