using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : PoolAble
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject dksl;
    void Start()
    {
        dksl = ObjectPoolManager.Instance.GetPoolObject("Monster_Student");
        Instantiate(dksl);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
