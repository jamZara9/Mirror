using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEventManager : Singleton<StoryEventManager>
{
    private IStoryEvent storyEvent;

    public void SetStoryEvent(IStoryEvent storyEvent)
    {
        Debug.Log("스토리이벤트매니저 : 이벤트 장착");
        this.storyEvent = storyEvent;
    }

    public void PlayStoryEvent()
    {
        Debug.Log("스토리이벤트매니저 : 이벤트 실행");
        storyEvent?.StoryEvent();
    }
}
