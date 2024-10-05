using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;     // 오브젝트 풀링 사용을 위한 네임 스페이스 추가

public class ObjectPoolManager : Singleton<ObjectPoolManager>, IManager
{
    [System.Serializable]
    private class ObjectInfo    // 오브젝트 풀로 관리할 오브젝트 정보 클래스
    {
        public string objectName;    // 오브젝트 이름
        public GameObject prefab;    // 오브젝트 풀에서 관리할 오브젝트(프리팹)
        public int count;            // 미리 생성할 오브젝트 갯수
    }

    public bool IsReady { get; private set; }    // 오브젝트풀 매니저 준비완료 체크용 변수

    [SerializeField]
    private ObjectInfo[] objectInfos = null;    // 오브젝트 풀로 관리할 오브젝트 정보 배열

    private string objectName;      // 생성할 오브젝트의 key값 지정을 위한 변수

    private Dictionary<string, IObjectPool<GameObject>> objectPoolDic = new Dictionary<string, IObjectPool<GameObject>>();  // 오브젝트 풀들을 관리할 딕셔너리

    private Dictionary<string, GameObject> objectDic = new Dictionary<string, GameObject>();    // 오브젝트 풀에서 오브젝트를 새로 생성할때 사용할 딕셔너리

    // void Awake()
    // {
    //     Init();     // 오브젝트 풀 초기 설정
    // }

    public void Initialize(string sceneName)
    {
        if(sceneName == SceneConstants.StartScene)
        {
            // 오브젝트 풀 초기 설정
            Init();
        }
    }

    /// <summary>
    /// 오브젝트 풀 초기 설정 함수
    /// </summary>
    private void Init()
    {
        IsReady = false;    // 오브젝트 풀 준비 상태로 변경

        for (int idx = 0; idx < objectInfos.Length; idx++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,   // 오브젝트 풀을 새로 생성
            OnDestroyPoolObject, true, objectInfos[idx].count, objectInfos[idx].count);

            if (objectDic.ContainsKey(objectInfos[idx].objectName))     // 이미 오브젝트 풀이 생성된 오브젝트인지 체크
            {
                Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", objectInfos[idx].objectName);
                return;     // 이미 생성되었다면 함수 종료
            }

            objectDic.Add(objectInfos[idx].objectName, objectInfos[idx].prefab);    // 오브젝트를 새로 생성하기 위한 딕셔너리에 추가
            objectPoolDic.Add(objectInfos[idx].objectName, pool);   // 오브젝트 풀 관리용 딕셔너리에 오브젝트 정보와 생성된 풀을 추가

            // 정해진 count에 맞춰 미리 오브젝트 생성
            for (int i = 0; i < objectInfos[idx].count; i++)
            {
                objectName = objectInfos[idx].objectName;
                PoolAble poolAbleGo = CreatePooledItem().GetComponent<PoolAble>();
                poolAbleGo.pool.Release(poolAbleGo.gameObject);
            }
        }

        Debug.Log("오브젝트풀링 준비 완료");
        IsReady = true;     // 오브젝트 풀 준비완료 상태로 변경
    }

    /// <summary>
    ///  오브젝트 풀로 관리할 오브젝트를 생성하는 함수
    /// </summary>
    /// <returns>생성된 오브젝트</returns>
    private GameObject CreatePooledItem()
    {
        GameObject poolObject = Instantiate(objectDic[objectName]);     // 오브젝트 딕셔너리에서 오브젝트 정보 가져옴
        poolObject.GetComponent<PoolAble>().pool = objectPoolDic[objectName];   // 가져온 오브젝트의 PoolAble 내의 오브젝트 풀에 사용할 오브젝트 풀 전달
        return poolObject;  // 세팅된 오브젝트 반환
    }

    /// <summary>
    /// 오브젝트 풀 내의 관리되고 있는 오브젝트를 대여하는 함수
    /// </summary>
    /// <param name="poolObject"></param>
    private void OnTakeFromPool(GameObject poolObject)
    {
        poolObject.SetActive(true);     // 오브젝트 활성화 상태로 변경
    }

    /// <summary>
    /// 오브젝트 풀 에서 대여된 오브젝트를 풀로 반환하는 함수
    /// </summary>
    /// <param name="poolObject"></param>
    private void OnReturnedToPool(GameObject poolObject)
    {
        poolObject.SetActive(false);    // 오브젝트 비활성화 상태로 변경
    }

    /// <summary>
    /// 오브젝트 풀 내의 오브젝트를 삭제하는 함수
    /// </summary>
    /// <param name="poolObject"></param>
    private void OnDestroyPoolObject(GameObject poolObject)
    {
        Destroy(poolObject);    // 오브젝트 삭제
    }

    /// <summary>
    /// 오브젝트 풀 내의 오브젝트를 대여해올 때 사용하는 함수
    /// </summary>
    /// <param name="gameobjectName"></param>
    /// <returns>대여되는 오브젝트</returns>
    public GameObject GetPoolObject(string gameobjectName)
    {
        objectName = gameobjectName;    // 대여할 오브젝트 key값 전달

        if (!objectDic.ContainsKey(gameobjectName))     // 오브젝트 풀에 등록된 오브젝트인지 체크 
        {
            Debug.LogFormat("{0} 오브젝트풀에 등록되지 않은 오브젝트입니다.", gameobjectName);
            return null;    // 등록되지 않았다면 null 반환
        }

        return objectPoolDic[gameobjectName].Get();     // 오브젝트 풀에 등록되어 있다면 오브젝트 반환
    }
}
