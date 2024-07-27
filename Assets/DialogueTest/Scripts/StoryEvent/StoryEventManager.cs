using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEventManager : Singleton<StoryEventManager>
{
    private Dictionary<int, StoryEventBase> _storyEventDictionary = new Dictionary<int, StoryEventBase>();
    private int _currentStoryEvent = 0;

    public void AddStoryEventDictionary(int index, StoryEventBase storyEventBase)
    {
        _storyEventDictionary.Add(index, storyEventBase);
    }

    public void PlayStoryEvent()
    {
        Debug.Log(_currentStoryEvent + "번째 스토리이벤트 실행");
        _storyEventDictionary[_currentStoryEvent].StoryEvent();
    }

    public void SuccessEvent()
    {
        ++_currentStoryEvent;
    }
}
