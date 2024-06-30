using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// UI 로드르 위한 네임스페이스
using TMPro;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    [Header("Mouse Settings")]
    private InputAction lookAction;             // Look 액션 참조
    public InputActionAsset  inputActionAsset;  // Input Action Asset 참조
    private Vector2 mousePosition;              // 마우스 위치


    [Header("Raycast Settings")]
    private Ray ray;                            // Raycast를 위한 Ray
    private RaycastHit hit;                     // Raycast를 통해 감지된 오브젝트
    public float maxDistance = 10f;             // Raycast 최대 거리

    [Header("UI Settings")]
    public TextMeshProUGUI objectNameText;      // 오브젝트 이름을 표시할 UI 텍스트
    public TextMeshProUGUI inputKeyText;        // 입력 키를 표시할 UI 텍스트

    private GameObject lastDetectedObject;      // 마지막으로 감지된 오브젝트

    void Awake()
    {
        // Input Action Asset에서 Look 액션 참조
        lookAction = inputActionAsset.FindActionMap("Player").FindAction("Look1");
        
    }
    void Update()
    {
        // Input System을 통해 마우스 위치를 업데이트
        mousePosition = lookAction.ReadValue<Vector2>();
        CheckForItem();

        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정
    }

    private void CheckForItem(){

        // Raycast를 통해 마우스 위치에 아이템이 있는지 확인 [ 필요에 따라서 카메라 위치로 변경 ]
        RaycastHit hit;
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ray = Camera.main.ScreenPointToRay(mousePosition);

        // Raycast를 발사하여 오브젝트를 감지
        if(Physics.Raycast(ray, out hit, maxDistance)){

            // 오브젝트가 "Item" 태그를 가지고 있는지 확인
            if(hit.collider.tag == "Item"){
                // Debug.Log("Item is detected");

                GameObject detectedObject = hit.collider.gameObject;                            // 마지막으로 감지된 오브젝트 저장
                objectNameText.text = detectedObject.name;                                      // UI에 오브젝트 이름 표시
                // inputKeyText.text = "Press 'E' to pick up";                                     // UI에 입력 키 표시

                objectNameText.text += " [E]";

                // 마지막으로 감지된 오브젝트를 업데이트
                lastDetectedObject = detectedObject;
                
                ItemManager.Instance.detectedItem = hit.collider.gameObject;                    // 감지된 아이템 저장
                ItemManager.Instance.detectedItem.GetComponent<BaseItem>().isPickable = true;   // 아이템을 획득 가능하도록 설정

                // lastDetectedObject.GetComponent<Outline>().OnMouseDown();                       // Outline 효과 활성화
            }else{
                // Debug.Log("Item is not detected");   
                // lastDetectedObject.GetComponent<Outline>().OnMouseUp();                         // Outline 효과 비활성화
                ResetDetectedItem();
            }
        }else{
            // lastDetectedObject.GetComponent<Outline>().OnMouseUp();                             // Outline 효과 비활성화
            ResetDetectedItem();
        }
    }

    /// <summary>
    /// 감지된 아이템을 초기화하는 함수
    /// </summary>
    private void ResetDetectedItem()
    {
        // UI를 초기화
        objectNameText.text = "";
        inputKeyText.text = "";

        // ItemManager 업데이트
        if (ItemManager.Instance.detectedItem != null)
        {
            ItemManager.Instance.detectedItem.GetComponent<BaseItem>().isPickable = false;
            ItemManager.Instance.detectedItem = null;
        }
    }

    // Input System 활성화
    private void OnEnable()
    {
        lookAction.Enable();
    }

    // Input System 비활성화
    private void OnDisable()
    {
        lookAction.Disable();
    }
}
