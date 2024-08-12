using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : Singleton<StoryManager>
{
    private Dictionary<int, StoryEventBase> _storyEventDictionary = new Dictionary<int, StoryEventBase>();
    private int _currentStoryEvent = 0;

    /// <summary>
    /// 매니저에 스토리 이벤트를 등록하는 함수.
    /// </summary>
    /// <param name="index">해당 이벤트의 순서 번호</param>
    /// <param name="storyEventBase">이벤트 로직 코드</param>
    public void AddStoryEventDictionary(int index, StoryEventBase storyEventBase)
    {
        _storyEventDictionary.Add(index, storyEventBase);
    }

    /// <summary>
    /// 순서에 맞는 이벤트 로직을 실행하는 함수.
    /// </summary>
    public void PlayStoryEvent()
    {
        Debug.Log(_currentStoryEvent + "번째 스토리이벤트 호출");
        _storyEventDictionary[_currentStoryEvent].StoryEvent();
    }

    public void SuccessEvent()
    {
        ++_currentStoryEvent;
    }
}
