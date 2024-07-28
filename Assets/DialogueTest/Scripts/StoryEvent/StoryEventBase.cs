using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StoryEventBase : MonoBehaviour
{
    [SerializeField] protected int myIndex;
    
    public abstract void StoryEvent();

    private void Awake()
    {
        Init();
        StoryEventManager.Instance.AddStoryEventDictionary(myIndex, this);
    }
    
    protected virtual void Init()
    {
        
    }
}
