using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolAble : MonoBehaviour
{
    public IObjectPool<GameObject> pool { get; set; }

    /// <summary>
    /// 오브젝트 풀에 오브젝트를 반환하는 함수
    /// </summary>
    public void ReleaseObject()
    {
        pool.Release(gameObject);
    }
}
