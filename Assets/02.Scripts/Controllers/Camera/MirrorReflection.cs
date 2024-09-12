using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 거울 화면을 표현하는 스크립트
/// </summary>
public class MirrorReflection : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;             // 플레이어의 카메라
    [SerializeField] private Camera mirrorCameraPrefab;     // 거울에서 사용할 카메라 프리팹
    private Camera mirrorCamera;                            // 각 거울에서 생성될 카메라 인스턴스
    private RenderTexture renderTexture;                    // 각 거울의 렌더 텍스처

    void Start()
    {
        // 거울마다 별도의 카메라 인스턴스와 렌더 텍스처 생성
        mirrorCamera = Instantiate(mirrorCameraPrefab);
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        mirrorCamera.targetTexture = renderTexture;

        // 거울 표면에 반사 화면을 적용
        GetComponent<Renderer>().material.mainTexture = renderTexture;

        // 거울의 x축 스케일이 양수일 때 음수로 변경하여 거울처럼 반사되도록 설정
        EnsureNegativeScaleX();
    }

    void LateUpdate()
    {
        // 플레이어 카메라와 거울의 위치 및 방향을 기준으로 반사 카메라 설정
        Vector3 mirrorNormal = transform.forward;                                       // 거울의 정면 방향

        mirrorCamera.transform.position = transform.position;

        // 거울 카메라의 회전 방향도 고려하여 반사된 방향으로 설정
        Vector3 reflectedDirection = Vector3.Reflect(mainCamera.transform.forward, mirrorNormal);
        mirrorCamera.transform.rotation = Quaternion.LookRotation(reflectedDirection, Vector3.up);
    }

    /// <summary>
    /// scale.x가 양수일 때 음수로 변경
    /// </summary>
    private void EnsureNegativeScaleX()
    {
        Vector3 scale = transform.localScale;
        if (scale.x > 0)
        {
            scale.x = -scale.x;
            transform.localScale = scale;
        }
    }

    private void OnDestroy()
    {
        // 거울 오브젝트가 파괴될 때 렌더 텍스처와 카메라 삭제
        if (mirrorCamera != null)
        {
            Destroy(mirrorCamera.gameObject);
        }
        if (renderTexture != null)
        {
            Destroy(renderTexture);
        }
    }
}
