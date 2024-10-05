
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : IManager
{
    // 중복 로드를 방지하기 위해 로드된 리소스를 저장하는 딕셔너리가 필요할 수도 있음
    // <리소스 경로, 리소스>
    private Dictionary<string, Object> _loadedResources = new();

    public void Initialize(string sceneName)
    {
    
    }

    /// <summary>
    /// 리소스를 로드하는 함수
    /// </summary>
    /// <typeparam name="T">로드할 오브젝트의 타입</typeparam>
    /// <param name="path">오브젝트 경로</param>
    public T LoadResource<T>(string path) where T : Object
    {
        // 이미 로드된 리소스가 있다면 해당 리소스를 반환
        if (_loadedResources.ContainsKey(path))
        {
            return _loadedResources[path] as T;
        }

        T resource = Resources.Load<T>(path);
        if (resource == null)
        {
            Debug.LogError($"파일이 존재하지 않습니다: {path}");
            return null;
        }

        _loadedResources.Add(path, resource);
        return resource;
    }

    /// <summary>
    /// [ 만일 씬 별 리소스를 폴더 단위로 관리하게 될 경우 사용할 수 있는 함수 ]
    /// 리소스를 로드하는 함수 (모든 리소스를 로드) 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T[] LoadAllResources<T>(string path) where T : Object
    {
        T[] resources = Resources.LoadAll<T>(path);
        if (resources.Length == 0)
        {
            Debug.LogError($"파일이 존재하지 않습니다: {path}");
            return null;
        }

        foreach (T resource in resources)
        {
            _loadedResources.Add(path, resource);
        }

        return resources;
    }
    
}
