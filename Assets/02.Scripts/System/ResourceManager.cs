
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
}
