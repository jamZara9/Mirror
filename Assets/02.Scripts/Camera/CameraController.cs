using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// UI 로드르 위한 네임스페이스
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 카메라 컨트롤러 클래스
/// 
/// <para>
/// author: Argonaut
/// </para>
/// </summary>
public class CameraController : MonoBehaviour
{

    [Header("Mouse Settings")]
    private Vector2 mousePosition;              // 마우스 위치
    private bool isCursorLocked = true;         // 마우스 커서 상태

    [Header("Raycast Settings")]
    private Ray ray;                            // Raycast를 위한 Ray
    private RaycastHit hit;                     // Raycast를 통해 감지된 오브젝트
    public float maxDistance = 10f;             // Raycast 최대 거리

    [Header("UI Settings")]
    public TextMeshProUGUI objectNameText;      // 오브젝트 이름을 표시할 UI 텍스트
    public GameObject detectedObject;           // 감지된 오브젝트
    private Outline _targetOutline;              // 아웃라인 캐싱

    /// <summary>
    /// 관측가능한 오브젝트 타입
    /// </summary>
    public enum DetectableType
    {
        None,
        Item,
        Weapon,
        Enemy
    }

    void Awake()
    {
        // 마우스 위치를 화면 중앙으로 초기화
        mousePosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        CheckForObject();       //@todo: 추후 마우스 입력이 있을 때만 실행하도록 변경
        UpdateCursorState();
    }

    /// <summary>
    /// 오브젝트를 감지하고 UI에 표시하는 함수
    /// 
    /// @todo: 추후 기능 분할 + 습득 키를 받아서 아이템을 획득하는 기능 추가
    /// </summary>
    private void CheckForObject()
    {

        // Raycast를 통해 마우스 위치에 아이템이 있는지 확인 [ 필요에 따라서 카메라 위치로 변경 ]
        ray = Camera.main.ScreenPointToRay(mousePosition);

        if(Physics.Raycast(ray, out hit, maxDistance)){
            // 관측된 오브젝트 타입 확인
            DetectableType type = GetDetectableType(hit.collider.tag);

            if(type != DetectableType.None){
                HandleDetectedObject(hit.collider.gameObject, type);
            }else{
                ResetDetectedItem();
            }
        }else{
            ResetDetectedItem();
        }
    }

    /// <summary>
    /// 감지된 오브젝트를 처리하는 메서드
    /// </summary>
    /// <param name="obj">감지된 오브젝트</param>
    /// <param name="type">감지된 타입</param>
    private void HandleDetectedObject(GameObject obj, DetectableType type){
        if(detectedObject != obj){
            ResetDetectedItem();    // 이전에 감지된 아이템을 초기화
            detectedObject = obj;   // 현재 감지된 오브젝트 저장

            // @Todo: 만약 관측된 적도 UI에 표시하고 싶다면 추가할 필요가 있음
            if(type == DetectableType.Item || type == DetectableType.Weapon){

                // UI에 오브젝트 이름 표시
                objectNameText.text = detectedObject.name + " [F]";                 

                // 아웃라인 활성화
                _targetOutline = detectedObject.GetComponent<Outline>();
                _targetOutline?.SetOutline(true);
            }

            if(type == DetectableType.Enemy){
                Debug.Log($"관측된 적: {detectedObject.name}");
            }
        }

    }

    /// <summary>
    /// 감지된 오브젝트의 태그를 기준으로 DetectableType을 반환하는 메서드
    /// </summary>
    /// <param name="tag">감지된 오브젝트의 태그</param>
    /// <returns>감지된 타입</returns>
    private DetectableType GetDetectableType(string tag){
        switch(tag){
            case "Item":
                return DetectableType.Item;
            case "Weapon":
                return DetectableType.Weapon;
            case "Enemy":
                return DetectableType.Enemy;
            default:
                return DetectableType.None;
        }
    }

    /// <summary>
    /// 감지된 아이템을 초기화하는 함수
    /// </summary>
    private void ResetDetectedItem()
    {
        if (detectedObject != null)
        {
            _targetOutline?.SetOutline(false);     // 아웃라인 비활성화
            detectedObject = null;
            objectNameText.text = string.Empty;   // UI를 초기화
        }
    }

    /// <summary>
    /// 커서 상태를 설정하는 함수
    /// </summary>
    /// <param name="state"></param>
    public void SetCursorState(bool state)
    {
        Cursor.visible = state;
        isCursorLocked = !state;
    }

    private void UpdateCursorState(){
        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
    }

}
