using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterStudent : MonoBehaviour,IDamage
{
    //   기본 상태값
    private Vector3 _startPosition;     // 학생(몬스터)의 시작 위치
    [SerializeField] 
    private float monsterHp = 20f;      // 학생(몬스터)의 체력
    private bool _isDamaged = false;        // 학생(몬스터)의 피격확인용 변수
    [SerializeField] 
    private float attackDistance = 2f;      // 학생(몬스터)의 공격범위
    [SerializeField]
    private float moveSpeed = 8f;       // 학생(몬스터)의 이동속도
    [SerializeField] 
    private float attackDelay = 2f;     // 학생(몬스터)의 공격 딜레이
    [SerializeField] 
    private float attackPower = 5f;     // 학생(몬스터)의 공격력
    private float _currentTime = 0f;    // 학생(몬스터)의 현재 공격시간 currentTime이 Delay보다 커진다면 공격 진행
    
    private Transform _player;          // 플레이어의 위치 값 받아오는 용도
    private Animator _animator;         // 학생(몬스터)의 애니메이터
    private NavMeshAgent _navMeshA;     // 학생(몬스터)의 네비매쉬매니저
    
    private Vector3 _randomPosition;
    private Vector3 _distance;
    

    MonsterState m_State;               // 학생(몬스터)의 현재 상태

    [SerializeField] private bool isMovingMonster = true;               // 학생(몬스터)가 움직일지 여부
    [SerializeField] private List<Vector3> moveDirectionList;           // 학생(몬스터)의 탐색 경로 지정리스트
    [SerializeField] private List<float> moveDirectionDelayList;        // 학생(몬스터)각 경로에서 몇초 동안 멈출지 지정리스트
    private bool _isWait = false;                                       // 학생(몬스터) 지금 대기중인지 확인용 변수
    private int _moveDirectionIndex = 0;                                // 학생(몬스터)현재 경로 인덱스
    [SerializeField] private bool DebugMode = false;                    // 학생(몬스터)의 탐색범위 가시화 할지 여부
    [Range(0f, 360f)] [SerializeField] private float ViewAngle = 0f;    // 학생(몬스터)의 탐색범위(시야각)
    [SerializeField] private float ViewRadius = 2f;                     // 학생(몬스터)의 탐색범위(반지름)
    [SerializeField] private LayerMask TargetMask;                      // 학생(몬스터)의 탐색대상 레이어
    [SerializeField] LayerMask ObstacleMask;                            // 학생(몬스터)의 탐색대상 레이어2

    private readonly List<Collider> _hitTargetList = new List<Collider>();// 탐지한 오브젝트 담을 리스트
    private readonly Vector3[] _directionCache = new Vector3[3];        // 지즈모 그릴 방향

    enum MonsterState       // 몬스터의 FSM
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Die
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
        // 지즈모 방향 구하기
        _directionCache[0] = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        _directionCache[1] = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        _directionCache[2] = AngleToDir(transform.eulerAngles.y);
        //---------------------------------------------------------------------
        // 초기화들
        _navMeshA = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _navMeshA.speed = moveSpeed;
        _startPosition = transform.position;
        _player = GameObject.FindWithTag("Player").transform;
        //---------------------------------------------------------------------
        
        _navMeshA.SetDestination(moveDirectionList[_moveDirectionIndex++]);
        m_State = MonsterState.Idle;
        
    }
    // 탐색 범위 가시화
    private void OnDrawGizmos() 
    {
        if (!DebugMode) return;
        Vector3 myPos = transform.position + Vector3.up * 0.5f - Vector3.back * 0.25f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        
        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, transform.forward * ViewRadius, Color.cyan);
    }
    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)// 스태이트머신
        {
            case MonsterState.Damaged:
                return;
            case MonsterState.Idle:
                Idle();
                break;
            case MonsterState.Move:
                Move();
                break;
            case MonsterState.Attack:
                Attack();
                break;
            case MonsterState.Die:
                Die();
                break;
        }
    }

   
    IEnumerator WaitIdle(int index) //      학생(몬스터)가 각 루트끝에 몇 초 동안 대기하고 다음 루트를 지정하는 함수
    {
        _isWait = true; //  기다림 시작
        _animator.SetTrigger("toIdle"); //                          애니메이션 변경
        yield return new WaitForSeconds(moveDirectionDelayList[index]); //   기다림
        _animator.SetTrigger("toWork"); //                          애니메이션 변경
        _navMeshA.SetDestination(moveDirectionList[_moveDirectionIndex]);   //  다음 루트 지정
        _moveDirectionIndex = (_moveDirectionIndex + 1) % moveDirectionList.Count;  
        _isWait = false; // 기다림 끝
    }
    
    void Idle()  //  학생(몬스터) 탐색
    {
        if (isMovingMonster && _navMeshA.remainingDistance <= _navMeshA.stoppingDistance) // 몬스터의 거리가 목적지와 가깝다면
        {
            if (!_isWait) //    대기 상태가 아니라면
            {
                StartCoroutine(WaitIdle(_moveDirectionIndex)); //   WaitIdle 함수 시작
            }
            Debug.Log(_moveDirectionIndex);
        }
        //  시야 적용 방식
        Vector3 myPos = transform.position + Vector3.up * 0.5f;     //      레이 시작 위치 
        _hitTargetList.Clear(); //                                          히트 리스트 초기화
        Collider[] Targets = Physics.OverlapSphere(myPos, ViewRadius, TargetMask);  //  원형 범위 내 레이어 검출

        if (Targets.Length == 0)    //                  아무것도 안 검출되면 리턴
            return;
        Debug.Log(Targets[0].name);
        foreach(Collider EnemyColli in Targets) //    검출되었을때 탐색범위(시야각) 내에 있는지 판별하는 부분
        {
            Vector3 targetPos = EnemyColli.transform.position + new Vector3(0, 2, 0);
            Vector3 targetDir = (targetPos - myPos).normalized;
            float targetAngle = Mathf.Acos(Vector3.Dot(transform.forward, targetDir)) * Mathf.Rad2Deg;
            if(targetAngle <= ViewAngle * 0.5 && !Physics.Raycast(myPos, targetDir, ViewRadius, ObstacleMask))
            {   //                                      검출된 오브젝트가 탐색범위(시야각)안에 있을때 레이를 발사하고 레이가 맞았을때
                Debug.Log("플레이어 감지");
                _hitTargetList.Add(EnemyColli);    //   리스트에 추가
                if (EnemyColli.gameObject.CompareTag("Player"))//   만약 레이를 맞은 오브젝트가 플레이어라면
                {
                    _animator.SetTrigger("toWork");     //      애니메이션 변경
                    m_State = MonsterState.Move;    //              학생(몬스터)의 상태를 Move로 변경
                }
                if (DebugMode) Debug.DrawLine(myPos, targetPos, Color.red); //  레이를 시각화 함
            }
        }

    }

    void Move()  //  학생(몬스터)추격
    {
        float SearchDistance = _isDamaged ? ViewRadius * 3 : ViewRadius * 2; 
        // 학생(몬스터)가 Damage로 Move가 됬는지 Idle에서 Move로 됬는지 만약 Damage라면 추격 범위가 탐색범위 * 3 아니면 탐색범위 * 2
        if (Vector3.Distance(_player.position, transform.position) > SearchDistance) // 플레이어와 학생(몬스터)의 거리가 탐색범위 보다 길다면
        {
            Debug.Log("d?" + SearchDistance);
            _navMeshA.SetDestination(_startPosition); // 시작 위치로 돌아감
            m_State = MonsterState.Idle;              // 애니메이션 변경 
            /*a_nim.SetTrigger("toWork");*/
            _isDamaged = false;
        }
        else if (Vector3.Distance(_player.position, transform.position) > attackDistance) 
            // 만약 플레이어의와의 거리가 공격범위보다 멀지만 탐색 벗어나는 범위보다 가깝다면
        {
            _navMeshA.destination = _player.position;   // 추격함 
        }
        else
        {   // 만약 플레이어와 학생(몬스터)의 거리가 공격 범위보다 가깝다면 
            m_State = MonsterState.Attack;// 학생(몬스터)의 상태를 Attak으로 변경
            _currentTime = attackDelay; // 처음 1회 공격 바로 시전
        }
    }

    void Attack()        //  학생(몬스터)의 플레이어 공격
    {                           //  만약 플레이어와 학생(몬스터)의 거리가 공격 범위 내라면
        if (Vector3.Distance(_player.position, transform.position) < attackDistance)
        {                       //  currentTime 카운트 시작
            _currentTime += Time.deltaTime;
            if (_currentTime > attackDelay)// currentTime이 attackDelay만큼 카운트 했다면 공격 진행
            {
                int randValue = Random.Range(0, 10);
                if (randValue < 5) //50%로 방향을 구함
                {
                    Debug.Log("공격");    
                }
                else
                {
                    Debug.Log("깨물기 공격");
                }
                _currentTime = 0;       // currentTime 초기화
            }
        }
        else
        {               //  플레이어와 학생(몬스터)의 거리가 공격 범위 보다 멀다면
            m_State = MonsterState.Move;    // 학생(몬스터)의 상태를 Move로 변경
            _currentTime = 0;   // currentTime 초기화
        }
    }

    public void Damage()    //  학생(몬스터)의 피격
    {
        Debug.Log("우와 이게 뭐야");
    }

    public void HitedMonster(int hitPower)  //  학생(몬스터)의 체력을 뺴는 함수
    {
        if (m_State == MonsterState.Damaged || m_State == MonsterState.Die) return; 
        if (monsterHp > 0)  
            //                                  체력이 0 or 죽은 상태가 아니라면
        {
            monsterHp -= hitPower;  // 학생(몬스터)의 체력을 뺌
            m_State = MonsterState.Damaged;// 학생(몬스터)의 상태를 Damaged 변경
            _isDamaged = true;             //피격 상태 
            StartCoroutine(WaitDamage());// WaitDamage 함수 호출
        }
    }

    IEnumerator WaitDamage()        // 피격 되는 시간동안 움직이지 못하게 하는 함수
    {
        
        yield return new WaitForSeconds(1f);//  1초동안 대기함
        m_State = MonsterState.Move;        //  학생(몬스터)의 상태를 Move 변경
    }

    void Die()   // 학생(몬스터)를 죽음처리(없앰)함
    {
        gameObject.SetActive(false);
    }
}