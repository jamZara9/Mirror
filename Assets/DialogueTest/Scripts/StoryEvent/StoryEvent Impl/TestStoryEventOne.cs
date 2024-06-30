using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStoryEvnetOne : MonoBehaviour, IStoryEvent
{
    [SerializeField] private GameObject storyEventTestTxt; 
    
    public void StoryEvent()
    {
        storyEventTestTxt.SetActive(true);
    }
}
