using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVDisplay : CameraFeedBase
{
    private enum CCTVType { Auto, Manual, Fix }; // CCTV의 타입

    [SerializeField] private Renderer screenRenderer;  // CCTV 화면을 표현할 렌더러
    [SerializeField] private Transform cctvTransform;    // CCTV 오브젝트
    [SerializeField] private CCTVType cctvType = CCTVType.Auto;        // CCTV 타입
    [SerializeField] private float rotationSpeed = 15.0f; // CCTV 회전 속도
    [SerializeField, Range(0.0f,45.0f)] private float rotationRange = 30.0f; // CCTV 회전 범위

    // 카메라 회전 관련 변수
    private float currentAngle = 0.0f;  // 현재 회전 각도
    private bool isRotatingRight = true; // 회전 방향
    private float rotationAmount = 0.0f; // 회전량
    private float rotationDirection = 1.0f; // 회전 방향
    

    void Start()
    {
        SetupCamera();                                  // 카메라 설정
        ApplyRenderToSurface(screenRenderer);           // 렌더 텍스처를 CCTV 화면에 적용
    }

    void LateUpdate()
    {
        // 매 프레임마다 카메라 위치와 방향을 업데이트하여 반사를 정확히 표현
        UpdateCameraPosition();
    }

    protected override void UpdateCameraPosition()
    {
        UpdateCameraRotation();                     // 카메라 회전

        // 피드 카메라를 거울의 위치에 맞춰 설정
        feedCamera.transform.position = transform.position;
    }

    /// <summary>
    /// CCTV의 회전을 업데이트하는 함수
    /// </summary>
    private void UpdateCameraRotation(){
        switch(cctvType){
            case CCTVType.Auto:
                AutoRotate();
                break;
            case CCTVType.Manual:
                // @TODO: CCTV를 수동 회전 / 필요 시 구현 예정
                break;
            case CCTVType.Fix:
                break;
        }
    }

    /// <summary>
    /// CCTV를 자동으로 회전시키는 함수
    /// </summary>
    private void AutoRotate(){
        rotationDirection = isRotatingRight ? 1.0f : -1.0f;                   // 회전 방향 설정
        rotationAmount = rotationSpeed * Time.deltaTime * rotationDirection;  // 회전량 계산
        cctvTransform.Rotate(Vector3.up, rotationAmount);              // 회전 적용
        currentAngle += rotationAmount;                                       // 현재 회전 각도 업데이트

        // 회전 각도가 범위를 넘으면 방향 전환
        if (currentAngle >= rotationRange || currentAngle <= -rotationRange)
        {
            isRotatingRight = !isRotatingRight;
        }
    }

}
