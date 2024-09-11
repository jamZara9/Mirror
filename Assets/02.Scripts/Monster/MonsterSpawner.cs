using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : PoolAble
{
    // Start is called before the first frame update
    [SerializeField] private List<Vector3> spawnerMoveDirectionList;           // 학생(몬스터)의 탐색 경로 지정리스트
    [SerializeField] private List<float> spawnerMoveDirectionDelayList;        // 학생(몬스터)각 경로에서 몇초 동안 멈출지 지정리스트
    
    [SerializeField] private GameObject movePositionGroup;

    [SerializeField] private string monsterType;
    
    [SerializeField] bool isMoving = true;

    [SerializeField] private List<GameObject> monsterList;                      //몬스터들이 담길 리스트

    [SerializeField]
    private GameObject[] creatMonster;
    
    MonsterFSM _monsterFsm;
    
    void Start()
    {
        creatMonster = Resources.LoadAll<GameObject>("Assets/03.Prefabs/Monster");
        // _monsterFsm = creatMonster.GetComponent<MonsterFSM>();
        // if (movePositionGroup != null)
        // {
        //     spawnerMoveDirectionList.Clear();
        //     for (int i = 0; i < movePositionGroup.transform.childCount; i++)
        //     {
        //         spawnerMoveDirectionList.Add(movePositionGroup.transform.GetChild(i).position);
        //     }
        // }
        // if (spawnerMoveDirectionDelayList.Count == 0)
        // {
        //     spawnerMoveDirectionDelayList.Clear();
        //     for (int i = 0; i < movePositionGroup.transform.childCount; i++)
        //     {
        //         spawnerMoveDirectionDelayList.Add(1);
        //     }
        // }
        //
        // _monsterFsm.moveDirectionList = spawnerMoveDirectionList;
        // _monsterFsm.moveDirectionDelayList = spawnerMoveDirectionDelayList;

    }
    void Update()
    {
            
    }
}
