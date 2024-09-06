using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;

    private float minLoadingTime = 5.0f; // 최소 로딩 시간 (초)

    private void Start()
    {
        // 초기 설정
        loadingBar.value = 0f;
        loadingText.text = "Loading... 0%";

        // PlayerPrefs에서 다음 씬 이름 읽기 (기본값: SceneConstants.PlaygroundA)
        string sceneToLoad = PlayerPrefs.GetString("NextScene", SceneConstants.PlaygroundA);

        // 로딩 씬에서 비동기 로드 시작
        StartCoroutine(LoadSceneAsync(sceneToLoad));

        // 비동기 로딩을 시뮬레이션
        // StartCoroutine(SimulateLoading());
    }

    // 씬을 비동기적으로 로드하는 코루틴
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // 로딩 창을 활성화
        loadingBar.gameObject.SetActive(true);
        loadingText.gameObject.SetActive(true);

        // 씬을 비동기적으로 로드
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            // 로딩 진행도 업데이트
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            float loadingProgress = Mathf.Clamp01(elapsedTime / loadingDuration);

            loadingBar.value = Mathf.Lerp(loadingBar.value, loadingProgress, Time.deltaTime);
            loadingText.text = $"Loading... {Mathf.RoundToInt(loadingBar.value * 100f)}%";

            // loadingBar.value = progress;
            // loadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100f)}%";

            yield return null;
        }

        // 로딩 완료 후 로딩 UI 비활성화
        loadingBar.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);
    }

    #region testCode
    private float loadingDuration = 20f; // 총 로딩 시간 (초)
    private float elapsedTime = 0f; // 경과 시간

    /// <summary>
    /// 로딩을 시뮬레이션하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SimulateLoading()
    {
        // 로딩 창을 활성화
        loadingBar.gameObject.SetActive(true);
        loadingText.gameObject.SetActive(true);

        while (elapsedTime < loadingDuration)
        {
            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / loadingDuration);

            // 로딩 바 업데이트
            loadingBar.value = progress;
            loadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100f)}%";

            yield return null;
        }

        // 로딩 완료 후 로딩 UI 비활성화
        loadingBar.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);

        // 다음 씬으로 전환
        string nextScene = PlayerPrefs.GetString("NextScene", SceneConstants.PlaygroundA);
        SceneManager.LoadScene(nextScene);
    }

    #endregion
}