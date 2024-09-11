using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : PoolAble
{
    // Start is called before the first frame update
    [Header("몬스터 이동 루트 지정")]
    
    [SerializeField] private List<float> spawnerMoveDirectionDelayList; // 학생(몬스터)각 경로에서 몇초 동안 멈출지 지정리스트
    [Header("몬스터 움직일지 여부")]
    [SerializeField] bool isMoving = true;
    [Header("몬스터 경로 그룹 오브젝트")]
    [SerializeField] private GameObject movePositionGroup;
    [Header("몬스터 리스트")]
    [SerializeField] private MonsterScriptable monsterList;
    [Header("몬스터 타입")]
    [SerializeField] private MonsterType monsterType;
    
    private GameObject _instantiateMonster;
    
    enum MonsterType
    {
        Student = 0,
        Mirror = 1
    }
    
    MonsterFSM _monsterFsm;
    
    void Start()
    {
        _instantiateMonster = monsterList.monsters[(int)monsterType];
        Instantiate(_instantiateMonster);
        _instantiateMonster.transform.position = transform.position;
        _monsterFsm = _instantiateMonster.GetComponent<MonsterFSM>();
        SettingInit();
    }
    void Update()
    {
            
    }

    void SettingInit() // 생성한 오브젝트의 이동경로, 멈추는 시간, 움직일지 여부를 초기화하는 함수입니다.
    {
        if (movePositionGroup != null)//이거 떔시 몬스터FSM에서는 딜레이 리스트 추가 안함
        {
            _monsterFsm.movePositionGroup = movePositionGroup;
            _monsterFsm.moveDirectionList.Clear();
            for (int i = 0; i < movePositionGroup.transform.childCount; i++)
            {
                _monsterFsm.moveDirectionList.Add(movePositionGroup.transform.GetChild(i).position);
            }
        }
        _monsterFsm.moveDirectionDelayList = spawnerMoveDirectionDelayList;
        _monsterFsm.isMovingMonster = isMoving;
    }
}
