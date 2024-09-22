using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorReflection : CameraFeedBase
{
    // [SerializeField] private Camera mirrorCameraPrefab;  // 거울에 사용할 카메라 프리팹

    private Vector3 mirrorNormal;       // 거울의 정면 방향
    private Vector3 reflectedDirection; // 반사된 방향

    void Start()
    {
        SetupCamera();                                  // 카메라 설정
        ApplyRenderToSurface(); // 렌더 텍스처를 거울 표면에 적용

        // 거울의 스케일을 반사 형태로 만들기 위해 x축 스케일을 음수로 설정
        EnsureNegativeScaleX();
    }

    void LateUpdate()
    {
        // 매 프레임마다 카메라 위치와 방향을 업데이트하여 반사를 정확히 표현
        UpdateCameraPosition();
    }

    /// <summary>
    /// 거울에 반사된 카메라 위치와 방향을 설정하는 함수.
    /// </summary>
    protected override void UpdateCameraPosition()
    {
        // 거울의 정면 방향 벡터
        mirrorNormal = transform.forward;
        // 메인 카메라의 방향을 거울의 정면에 대해 반사된 방향으로 계산
        reflectedDirection = Vector3.Reflect(mainCamera.transform.forward, mirrorNormal);
        
        // 피드 카메라를 거울의 위치에 맞춰 설정
        feedCamera.transform.position = transform.position;
        // 반사된 방향으로 카메라 회전
        feedCamera.transform.rotation = Quaternion.LookRotation(reflectedDirection, Vector3.up);
    }

    /// <summary>
    /// 거울의 x축 스케일이 양수일 경우, 음수로 변경하여 올바른 반사 효과를 제공.
    /// </summary>
    private void EnsureNegativeScaleX()
    {
        Vector3 scale = transform.localScale;
        if (scale.x > 0) scale.x = -scale.x;
        transform.localScale = scale;
    }
}
