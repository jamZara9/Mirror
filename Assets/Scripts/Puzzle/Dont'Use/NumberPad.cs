using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberPad : MonoBehaviour
{
    public Camera myCam;
    public LayerMask button;

    [SerializeField] private long nowAnswer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            
            Ray myRay = myCam.ScreenPointToRay(mousePosition);
            
            RaycastHit raycastHit;
            
            bool weHitSomething = Physics.Raycast(myRay, out raycastHit, button);
            
            if (weHitSomething)
            {
                Debug.Log(raycastHit.transform.name);
                PuzzleClick(raycastHit.transform.name);
            }
        }
    }

    void PuzzleClick(string input)
    {
        if (input == "*" || input == "#")
        {
            if (nowAnswer == 72427)
            {
                Debug.Log("Clear");
            }
            else
            {
                nowAnswer = 0;
                Debug.Log("Error");
            }
        }
        else if (int.Parse(input) < 10 && int.Parse(input) >= 0)
        {
            nowAnswer = nowAnswer * 10 + int.Parse(input);
            Debug.Log("현재 답 : " + nowAnswer);
        }
    }
}
