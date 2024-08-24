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

    public GameObject detectedItem;             // 감지된 아이템

    void Awake()
    {
        // 마우스 위치를 화면 중앙으로 초기화
        mousePosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        CheckForItem();
        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;   // 마우스 커서 해제
        }
    }


    /// <summary>
    /// 아이템 감지하고 UI에 표시하는 함수
    /// 
    /// @todo: 추후 기능 분할 + 습득 키를 받아서 아이템을 획득하는 기능 추가
    /// </summary>
    private void CheckForItem()
    {

        // Raycast를 통해 마우스 위치에 아이템이 있는지 확인 [ 필요에 따라서 카메라 위치로 변경 ]
        ray = Camera.main.ScreenPointToRay(mousePosition);

        // Raycast를 발사하여 오브젝트를 감지
        if (Physics.Raycast(ray, out hit, maxDistance))
        {

            string hitTag = hit.collider.tag;   // 감지된 오브젝트의 태그

            if (hitTag == "Item" || hitTag == "Weapon")
            {
                if (detectedItem != hit.collider.gameObject)
                {
                    ResetDetectedItem();    // 이전에 감지된 아이템을 초기화

                    detectedItem = hit.collider.gameObject;                    // 마지막으로 감지된 오브젝트 저장
                    objectNameText.text = detectedItem.name + " [F]";          // UI에 오브젝트 이름 표시

                    var itemOutline = detectedItem.GetComponent<Outline>();
                    itemOutline?.SetOutline(true);     // 아웃라인 활성화

                    if (hitTag == "Item")
                    {
                        detectedItem.GetComponent<IInventoryItem>().IsPickable = true;   // 아이템을 획득 가능하도록 설정
                    }
                }
            }else{
                ResetDetectedItem();
            }
        }
        else
        {
            ResetDetectedItem();
        }
    }

    /// <summary>
    /// 감지된 아이템을 초기화하는 함수
    /// </summary>
    private void ResetDetectedItem()
    {
        if (detectedItem != null)
        {
            var itemOutline = detectedItem.GetComponent<Outline>();
            itemOutline?.SetOutline(false);     // 아웃라인 비활성화

            detectedItem = null;
            objectNameText.text = "";   // UI를 초기화
        }
    }

    /// <summary>
    /// 커서 상태를 설정하는 함수
    /// </summary>
    /// <param name="state"></param>
    public void SetCursorState(bool state)
    {
        // Debug.Log("Cursor State: " + state);
        Cursor.visible = state;
        isCursorLocked = !state;
        // Debug.Log("Cursor Lock State: " + isCursorLocked);
        // Debug.Log("Cursor Visible State: " + Cursor.visible);
    }
}
