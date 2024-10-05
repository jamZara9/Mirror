using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForStory : MonoBehaviour
{
    private void Awake()
    {
        DialogueManager.Instance.Initialize(SceneConstants.PlaygroundA);
    }
}
