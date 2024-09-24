using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬을 로드하는 클래스
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// 비동기적으로 씬을 로드하는 코루틴
    /// </summary>
    /// <param name="sceneName">로드하고자 하는 씬 이름</param>
    /// <returns></returns>
    public IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
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
}
