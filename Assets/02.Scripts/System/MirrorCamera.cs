using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
    [SerializeField] private Transform _mirrorObject; // 거울 오브젝트
    [SerializeField] private Camera _mirrorCamera; // 거울을 렌더링할 카메라

    private void Start()
    {
        SetupMirrorCamera();
    }

    // 카메라 좌우 반전 함수
    private void SetupMirrorCamera()
    {
        if (_mirrorObject == null || _mirrorCamera == null)
        {
            Debug.LogError("MirrorObject / MirrorCamera가 할당되지 않았습니다.");
            return;
        }

        // 카메라의 위치를 거울 오브젝트의 정중앙으로 설정
        _mirrorCamera.transform.position = _mirrorObject.position + (_mirrorObject.forward * Vector3.Distance(_mirrorCamera.transform.position, _mirrorObject.position));

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
