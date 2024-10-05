using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingController : MonoBehaviour
{
    public Slider loadingBar;               // 로딩 바
    public TextMeshProUGUI loadingText;     // 로딩 텍스트
    public CanvasGroup canvasGroup;         // 캔버스 그룹

    [Header("Loading Settings")]
    [SerializeField] private float minLoadingTime = 5.0f; // 최소 로딩 시간 (초)
    private float elapsedTime = 0f; // 경과 시간

    private void Start()
    {
        // 초기 설정
        loadingBar.value = 0f;
        loadingText.text = "Loading... 0%";

        // PlayerPrefs에서 다음 씬 이름 읽기 (기본값: SceneConstants.PlaygroundA)
        string sceneToLoad = PlayerPrefs.GetString("NextScene", SceneConstants.PlaygroundA);

        // 로딩 씬에서 비동기 로드 시작
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    // 씬을 비동기적으로 로드하는 코루틴
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // 로딩 창을 활성화
        loadingBar.gameObject.SetActive(true);
        loadingText.gameObject.SetActive(true);

        // 씬을 비동기적으로 로드
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;  // 씬 로드를 완료하지 않도록 설정

        while (!operation.isDone)
        {
            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            // 로딩 진행도 업데이트
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = Mathf.Lerp(loadingBar.value, progress, Time.deltaTime);
            loadingText.text = $"Loading... {Mathf.RoundToInt(loadingBar.value * 100f)}%";
            
            // 최소 로딩 시간이 지나고 씬 로드가 완료되었을 때
            if (progress >= 1f && elapsedTime >= minLoadingTime)
            {
                operation.allowSceneActivation = true;  // 씬 전환 허용
            }
    
            yield return null;
        }

        // 로딩 완료 후 로딩 UI 비활성화
        loadingBar.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);
    }
}