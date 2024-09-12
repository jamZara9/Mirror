using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 출력을 다루는 추상 클래스
/// </summary>
public abstract class CameraFeedBase : MonoBehaviour
{   
    [Header("Camera Feed Settings")]
    [SerializeField] protected Camera mainCamera;              // 플레이어의 메인 카메라
    [SerializeField] protected Camera feedCameraPrefab;        // 거울이나 CCTV에서 사용할 카메라 프리팹
    [SerializeField] protected LayerMask layerMask;            // 카메라가 레이캐스트할 레이어 마스크
    [SerializeField] protected bool isCameraToChild = false;   // 카메라를 자식 오브젝트에 붙일지 여부


    protected Camera feedCamera;                   // 실제 화면을 그릴 카메라
    protected RenderTexture renderTexture;         // 화면을 그릴 렌더 텍스처

    protected virtual void SetupCamera()
    {
        SetupCamera(mainCamera, feedCameraPrefab);
    }

    /// <summary>
    /// 카메라를 초기화하는 함수
    /// </summary>
    /// <param name="mainCamera">플레이어의 메인 카메라</param>
    /// <param name="feedCameraPrefab">거울이나 CCTV에서 사용할 카메라 프리팹</param>
    protected virtual void SetupCamera(Camera mainCamera, Camera feedCameraPrefab)
    {
        this.mainCamera = mainCamera;

        // // 프리팹을 인스턴스화 하여 피드 카메라로 사용
        // feedCamera = Instantiate(feedCameraPrefab);

        // 카메라를 자식 오브젝트에 붙일지 여부에 따라 부모 오브젝트 설정
        feedCamera = isCameraToChild ? Instantiate(feedCameraPrefab, transform) : Instantiate(feedCameraPrefab);

        // 화면 크기에 맞는 렌더 텍스처를 생성
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        // 피드 카메라가 해당 렌더 텍스처에 그림을 그리도록 설정
        feedCamera.targetTexture = renderTexture;

        // layerMask에 설정된 레이어만 렌더링되도록 설정
        feedCamera.cullingMask = layerMask;
    }

    protected void ApplyRenderToSurface()
    {
        ApplyRenderToSurface(GetComponent<Renderer>());
    }

    /// <summary>
    /// 생성된 렌더 텍스처를 지정된 표면(거울이나 모니터 등)에 적용하는 함수.
    /// </summary>
    /// <param name="surfaceRenderer">거울 또는 CCTV 화면을 표현할 렌더러</param>
    protected void ApplyRenderToSurface(Renderer surfaceRenderer)
    {
        // 표면의 재질 텍스처를 생성한 렌더 텍스처로 설정
        surfaceRenderer.material.mainTexture = renderTexture;
    }

    /// <summary>
    /// 카메라의 위치 및 방향을 업데이트하는 추상 메서드.
    /// 상속받는 클래스에서 구체적으로 구현해야 함.
    /// </summary>
    protected abstract void UpdateCameraPosition();

    /// <summary>
    /// 오브젝트가 파괴될 때 호출되는 함수.
    /// 생성된 카메라와 렌더 텍스처를 정리.
    /// </summary>
    private void OnDestroy()
    {
        // 생성된 피드 카메라가 있으면 삭제
        if (feedCamera != null) Destroy(feedCamera.gameObject);
        // 렌더 텍스처가 있으면 삭제
        if (renderTexture != null) Destroy(renderTexture);
    }
}
