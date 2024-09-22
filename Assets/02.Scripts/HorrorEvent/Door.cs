using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[Serializable]
struct DoorStruct
{
    public Vector3 rotation;
    public float duration;
}

public class Door : MonoBehaviour, IInteractionable
{
    private bool _isOpen = false;

    [SerializeField] private DoorStruct[] doorStruct;

    private void Start()
    {
        // 임시로 상호작용 테스트.
        Interaction();
    }

    public void Interaction()
    {
        if (!_isOpen)
        {
            _isOpen = true;
            StartCoroutine(Rotate());
        }
        else
        {
            _isOpen = false;
            StartCoroutine(Rotate());
        }
    }
    
    IEnumerator Rotate()
    {
        foreach (var doorInfo in (_isOpen ? doorStruct : doorStruct.Reverse()))
        {
            float currentTime = 0;
            Vector3 startVector = transform.eulerAngles;
            
            while (startVector != doorInfo.rotation && currentTime < doorInfo.duration)
            {
                currentTime += Time.deltaTime;
                transform.eulerAngles = Vector3.Lerp(startVector,doorInfo.rotation, currentTime / doorInfo.duration);
                yield return null;
            }
        }
    }
}
