using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StoryEventBase : MonoBehaviour
{
    // 자신의 스토리 진행 순서.
    [SerializeField] protected int myIndex;
    
    // 이벤트 로직이 작성되는 추상 함수.
    public abstract void StoryEvent();

    private void Awake()
    {
        Init();
        // 시작 시, 자신의 순서 번호, 로직을 매니저의 딕셔너리에 추가. 
        StoryManager.Instance.AddStoryEventDictionary(myIndex, this);
    }
    
    /// <summary>
    /// 하위 클래스에서 Awake 대신 사용하는 함수.
    /// </summary>
    protected virtual void Init()
    {
        
    }
}
