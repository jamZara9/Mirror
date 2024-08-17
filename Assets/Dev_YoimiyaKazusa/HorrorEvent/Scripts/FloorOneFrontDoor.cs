using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorOneFrontDoor : MonoBehaviour, IInteractionable
{
    [SerializeField] private StoryScene myStoryScene;

    private bool _isOpen = false;
    
    [SerializeField] private float rotateDuration;

    [SerializeField] private Vector3 originRotion;
    [SerializeField] private Vector3 openRotation;

    private GameController _gameController;

    private void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
    }

    private void Start()
    {
        Interaction();
    }

    public void Interaction()
    {
        _gameController.PlayScene(myStoryScene);
        
        if (!_isOpen)
        {
            _isOpen = true;
            StartCoroutine(Rotate(originRotion, openRotation));
        }
        else
        {
            _isOpen = false;
            StartCoroutine(Rotate(openRotation, originRotion));
        }
    }

    IEnumerator Rotate(Vector3 startVector, Vector3 endVector)
    {
        float currentTime = 0;
        
        while (currentTime < rotateDuration)
        {
            currentTime += Time.deltaTime;
            transform.eulerAngles = Vector3.Lerp(startVector,endVector, currentTime / rotateDuration);
            yield return null;
        }
    }
}
