using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup uiCanvasGroup; // UI 요소의 CanvasGroup 컴포넌트 [ 마우스 클릭 시 보여줄 UI]

    private bool isUIVisible = false;   // UI가 보이는지 여부
    private float loadUITime = 1.5f;    // UI가 보이는 시간
    void Start()
    {
        // 초기 설정: uiTitle은 보이지 않게, uiCanvasGroup은 투명하게 설정
        InitializeUI();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isUIVisible){
            isUIVisible = true;
            StartCoroutine(LoadUI());   //  UI를 나타내는 코루틴 함수 호출
        }
        
    }

    public void PlayStart()
    {
        PlayerPrefs.SetString("NextScene", SceneConstants.PlaygroundA);
        // SceneManager.LoadScene("PlaygroundLoading");
        // 로딩 화면을 비동기로 로드 시작
        StartCoroutine(LoadLoadingScene());
    }

    private IEnumerator LoadLoadingScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneConstants.LoadingScene);
        asyncLoad.allowSceneActivation = false;

        // 로딩 씬이 준비될 때까지 대기
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                // 로딩이 완료되면 씬을 활성화
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    /// <summary>
    /// UI 초기 설정
    /// </summary>
    private void InitializeUI()
    {
        uiCanvasGroup.alpha = 0f;
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;
    }

    // UI를 서서히 나타내는 코루틴 함수
    private IEnumerator LoadUI()
    {
        float elapsedTime = 0f; // 경과 시간

        // uiTitle을 서서히 투명하게
        while (elapsedTime < loadUITime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f; // 경과 시간 초기화

        // uiCanvasGroup을 서서히 나타내기
        while (elapsedTime < loadUITime)
        {
            uiCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / loadUITime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // uiCanvasGroup 완전히 나타난 후에 상호작용 및 레이캐스트 활성화
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;
    }

}
