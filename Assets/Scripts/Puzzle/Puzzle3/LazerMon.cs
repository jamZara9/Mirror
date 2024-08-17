using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(LineRenderer))]
public class LazerMon : MonoBehaviour
{
    [Header("초기 세팅")]
    public LayerMask reflectLayerMask;      // 레이저를 반사할 Layer
    public LayerMask defaultLayerMask;      // 반사하지 않고 흡수할 Layer
    public LayerMask clearObj;              // 레이저가 닿았을 때 클리어 될 Layer
    public float defaultLength = 50;        // 레이저의 길이
    public float reflectNum = 20;           // 반사 가능 횟수

    private LineRenderer lineRenderer;      // 레이저 표시용 LineRenderer 변수
    private RaycastHit   hit;               // 오브젝트 충돌 체크용 Raycast 변수
    
    
    void Start()
    {
        // LineRenderer 컴포넌트 받아옴
        lineRenderer = GetComponent<LineRenderer>();   
    }

    void Update()
    {
        // 레이저 반사 함수 실행
        ReflectLazer(); 
    }
    
    // 레이저 반사 함수
    void ReflectLazer()
    {
        // Ray를 생성
        var ray = new Ray(transform.position, transform.forward);

        // LineRenderer의 다음 도착 지점을 1로 설정
        lineRenderer.positionCount = 1;
        // LineRenderer 시작점 지정
        lineRenderer.SetPosition(0, transform.position);

        // Raycast의 길이 변수를 defaultLength로 지정
        var resetLen = defaultLength;

        // 반사되는 횟수만큼 반복
        for (int i = 0; i < reflectNum; i++)
        {
            // LineRenderer의 다음 지점을 추가
            lineRenderer.positionCount += 1;
            
            // Raycast가 reflectLayerMask를 가진 오브젝트에 충돌했을 시
            if (Physics.Raycast(ray.origin, ray.direction, out hit, resetLen, reflectLayerMask))
            {
                // LineRenderer를 이전 위치에서 충돌 지점까지 그림
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                
                // ray를 충돌 지점에서 반사각만큼 회전한 방향으로 재생성
                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
            }
            // Raycast가 defaultLayerMask를 가진 오브젝트에 충돌했을 시
            else if (Physics.Raycast(ray.origin, ray.direction, out hit, resetLen, defaultLayerMask))
            {
                // LineRenderer를 이전 위치에서 충돌 지점까지 그림
                lineRenderer.SetPosition(lineRenderer.positionCount -1, hit.point);
            }
            // Raycast가 ClearObj를 가진 오브젝트에 충돌했을 시
            else if (Physics.Raycast(ray.origin, ray.direction, out hit, resetLen, clearObj))
            {
                // LineRenderer를 이전 위치에서 충돌 지점까지 그림
                lineRenderer.SetPosition(lineRenderer.positionCount -1, hit.point);
                
                Debug.Log("Clear");     // 클리어 확인용
            }
            else // 그 이외
            {
                // LineRenderer를 (lineRenderer.positionCount - 1) 번의 충돌 지점에서 
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + (ray.direction * resetLen));
            }
        }
    }
}
