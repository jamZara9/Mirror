using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
    [SerializeField] private Transform _mirrorObject; // 거울 오브젝트
    [SerializeField] private Camera _mirrorCamera; // 거울을 렌더링할 카메라

    private void Start()
    {
        // 카메라가 거울에 좌우 반전된 상태로 보이도록 설정
        MirrorCameraFlip();
    }

    // 카메라 좌우 반전 함수
    private void MirrorCameraFlip()
    {
        // 카메라의 로컬 스케일을 좌우 반전 (X축에 -1)
        Vector3 cameraScale = _mirrorCamera.transform.localScale;
        cameraScale.x *= -1; // X축 반전
        _mirrorCamera.transform.localScale = cameraScale;

        // Clipping 방지하고 제대로 렌더링되도록 카메라의 Culling 설정
        _mirrorCamera.ResetWorldToCameraMatrix();
        _mirrorCamera.ResetProjectionMatrix();
        _mirrorCamera.projectionMatrix = _mirrorCamera.projectionMatrix * Matrix4x4.Scale(new Vector3(-1, 1, 1));
    }
}
