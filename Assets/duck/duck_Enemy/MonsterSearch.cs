using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private bool DebugMode = false;
    [Range(0f, 360f)] [SerializeField] private float ViewAngle = 0f;
    [SerializeField] private float ViewRadius = 2f;
    [SerializeField] private LayerMask TargetMask;
    [SerializeField] LayerMask ObstacleMask;
    

    private readonly List<Collider> hitTargetList = new List<Collider>();
    private readonly Vector3[] directionCache = new Vector3[3];

    private void Start()
    {
        directionCache[0] = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        directionCache[1] = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        directionCache[2] = AngleToDir(transform.eulerAngles.y);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            transform.Rotate(0,2,0);
        }
    }

    private void OnDrawGizmos()
    {
        if (!DebugMode) return;
        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);
        
        // float lookingAngle = transform.eulerAngles.y;  //캐릭터가 바라보는 방향의 각도
        // Vector3 lookDir = AngleToDir(lookingAngle);
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        

        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, transform.forward * ViewRadius, Color.cyan);
        //--------------------------------------------
        
        hitTargetList.Clear();
        Collider[] Targets = Physics.OverlapSphere(myPos, ViewRadius, TargetMask);

        if (Targets.Length == 0) return;
        Debug.Log(Targets[0].name);
        foreach(Collider EnemyColli in Targets)
        {
            Vector3 targetPos = EnemyColli.transform.position;
            Vector3 targetDir = (targetPos - myPos).normalized;
            float targetAngle = Mathf.Acos(Vector3.Dot(transform.forward, targetDir)) * Mathf.Rad2Deg;
            if(targetAngle <= ViewAngle * 0.5 && !Physics.Raycast(myPos, targetDir, ViewRadius, ObstacleMask))
            {
                Debug.Log("됐나요?");
                hitTargetList.Add(EnemyColli);
                if (EnemyColli.gameObject.CompareTag("Player"))
                {
                    Debug.Log("아니 몰라");
                }
                if (DebugMode) Debug.DrawLine(myPos, targetPos, Color.red);
            }
        }

    }

    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }
}
