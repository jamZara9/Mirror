using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CylinderSet : MonoBehaviour
{
    [Tooltip("실린더 순서대로 넣기")]
    public SpinCylinder[] spinCylinder;
    
    [Tooltip("최대 회전 횟수")]
    public int[] cylinderSpinSet;
    
    [Tooltip("각 실린더 별 답")]
    public int[] puzzleAnswer;
    
    [Tooltip("각 실린더 별 현재 답")]
    // [HideInInspector]
    public int[] puzzleNowAnswer;

    private bool _open;

    public float speed;
    
    public Camera myCam;
    
    // Start is called before the first frame update
    void Awake()
    {
        puzzleNowAnswer = new int[puzzleAnswer.Length];
        for (int i = 0; i < spinCylinder.Length; i++)
        {
            spinCylinder[i] = spinCylinder[i].GetComponent<SpinCylinder>();
            spinCylinder[i].myNum = i;
            puzzleNowAnswer[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (puzzleAnswer.SequenceEqual(puzzleNowAnswer))
        {
            _open = true;
            Debug.Log("Clear!");
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            
            Ray myRay = myCam.ScreenPointToRay(mousePosition);
            
            RaycastHit raycastHit;
            
            bool weHitSomething = Physics.Raycast(myRay, out raycastHit);
            
            if (weHitSomething)
            {
                Debug.Log(raycastHit.transform.name);
                raycastHit.transform.GetComponent<SpinCylinder>().PuzzleClick();
            }
        }
    }
}
