using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;

/// <summary>
/// 게임의 전반적인 관리를 담당하는 클래스
/// </summary>
public class GameManager : Singleton<GameManager>, IManager
{
    #region Manager
    public ItemManager itemManager;
    public CameraController cameraController;         // 카메라 컨트롤러      x
    public UIController_Test uiController;            // UI 컨트롤러         x
    public PlayerStatus playerStatus;                 // 플레이어 상태        x
    public PlayerInventory playerInventory;           // 플레이어 인벤토리     x
    public WeaponManager weaponManager;               // 무기 매니저
    public InventoryManager inventoryManager;         // 인벤토리 매니저
    // public InputManager inputManager;                 // 입력 매니저
    #endregion

    [Header("Managers")]
    public static readonly UIManager uiManager = new();                   // UI 매니저


    public static readonly InputManager inputManager = new();             // 입력 매니저
    public static readonly ResourceManager resourceManager = new();       // 리소스 매니저
    public static readonly SystemManager systemManager = new();           // 시스템 매니저
    public static readonly AudioManager audioManager = new();             // 오디오 매니저

    // public static readonly CameraManager cameraManager = new();           // 카메라 매니저
    // public static readonly PlayerManager playerManager = new();           // 플레이어 매니저
    // public static readonly DialogueManager dialogueManager = new();       // 대화 매니저
    // public static readonly MonsterManager monsterManager = new();         // 몬스터 매니저
    // public static readonly ObjectPoolManager objectPoolManager = new();   // 오브젝트 풀 매니저

    public string CurrentScene {
        get {
            return SceneManager.GetActiveScene().name;
        }
    }

    void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // GameManager가 씬 변경 시에도 파괴되지 않도록 설정

        AudioSource bgmSource = GameObject.Find("BGM").GetComponent<AudioSource>();
        audioManager.SetBGMSource(bgmSource);
    }


    void OnEnable()
    {
        // 씬 로드 이벤트에 메서드 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 씬 로드 이벤트에서 메서드 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때 호출되는 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Initialize 메서드 호출
        Initialize(CurrentScene);
    }
    
    public void Initialize(string sceneName)
    {
        inputManager.Initialize(sceneName);

    }

}
