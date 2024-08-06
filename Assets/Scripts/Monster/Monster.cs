using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour,IMonster
{
    private Vector3 _startPosition;
    [SerializeField] 
    private float monsterHp = 20f;
    private bool _isDamaged = false;
    [SerializeField] 
    private float attackDistance = 2f;
    [SerializeField]
    private float moveSpeed = 8f;

    private float _currentTime = 0f;
    [SerializeField] 
    private float attackDelay = 2f;
    [SerializeField] 
    private float attackPower = 5f;
    private Transform _player;


    private Animator a_nim;
    
    private Vector3 _randomPosition;
    private Vector3 distan;
    private NavMeshAgent _navMeshA;

    MonsterState m_State;

    [SerializeField] private bool isMovingMonster = true;
    [SerializeField] private List<Vector3> moveDirectionList;
    [SerializeField] private List<float> moveDirectionDelayList;
    private bool isWait = false;
    private int _moveDirectionIndex = 0;
    
    //[SerializeField] 
    //private float fov = 90f; // 시야 각
    //[SerializeField] 
    //private float rayAngle = 5f; // 시야 각
    
    //-----------------------------------------
    
    [SerializeField] private bool DebugMode = false;
    [Range(0f, 360f)] [SerializeField] private float ViewAngle = 0f;
    [SerializeField] private float ViewRadius = 2f;
    [SerializeField] private LayerMask TargetMask;
    [SerializeField] LayerMask ObstacleMask;

    private readonly List<Collider> hitTargetList = new List<Collider>();
    private readonly Vector3[] directionCache = new Vector3[3];

    enum MonsterState
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
        a_nim = GetComponentInChildren<Animator>();
        directionCache[0] = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        directionCache[1] = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        directionCache[2] = AngleToDir(transform.eulerAngles.y);
        //---------------------------------------------------------------------
        _navMeshA = GetComponent<NavMeshAgent>();
        _startPosition = transform.position;
        _navMeshA.SetDestination(moveDirectionList[_moveDirectionIndex++]);
        m_State = MonsterState.Idle;
        _player = GameObject.FindWithTag("Player").transform;
    }

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
        switch (m_State)
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

   
    IEnumerator WaitIdle(int index)
    {
        isWait = true;
        a_nim.SetTrigger("toIdle");
        yield return new WaitForSeconds(moveDirectionDelayList[index]);
        a_nim.SetTrigger("toWork");
        _navMeshA.SetDestination(moveDirectionList[_moveDirectionIndex]);
        _moveDirectionIndex = (_moveDirectionIndex + 1) % moveDirectionList.Count;
        isWait = false;
    }
    
    public void Idle()
    {
        
        if (isMovingMonster && _navMeshA.remainingDistance <= _navMeshA.stoppingDistance)
        {
            if (!isWait)
            {
                StartCoroutine(WaitIdle(_moveDirectionIndex));
            }
            Debug.Log(_moveDirectionIndex);
        }
        //시야 적용 방식
        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        hitTargetList.Clear();
        Collider[] Targets = Physics.OverlapSphere(myPos, ViewRadius, TargetMask);

        if (Targets.Length == 0)
            return;
        Debug.Log(Targets[0].name);
        foreach(Collider EnemyColli in Targets)
        {
            Vector3 targetPos = EnemyColli.transform.position + new Vector3(0, 2, 0);
            Vector3 targetDir = (targetPos - myPos).normalized;
            float targetAngle = Mathf.Acos(Vector3.Dot(transform.forward, targetDir)) * Mathf.Rad2Deg;
            if(targetAngle <= ViewAngle * 0.5 && !Physics.Raycast(myPos, targetDir, ViewRadius, ObstacleMask))
            {
                Debug.Log("플레이어 감지");
                hitTargetList.Add(EnemyColli);
                if (EnemyColli.gameObject.CompareTag("Player"))
                {
                    a_nim.SetTrigger("toWork");
                    m_State = MonsterState.Move;
                }
                if (DebugMode) Debug.DrawLine(myPos, targetPos, Color.red);
            }
        }

    }

    public void Move()
    {
        float SearchDistance = _isDamaged ? ViewRadius * 3 : ViewRadius * 2;
        if (Vector3.Distance(_player.position, transform.position) > SearchDistance)
        {
            Debug.Log("d?" + SearchDistance);
            _navMeshA.SetDestination(_startPosition);
            m_State = MonsterState.Idle;
            /*a_nim.SetTrigger("toWork");*/
            _isDamaged = false;
        }
        else if (Vector3.Distance(_player.position, transform.position) > attackDistance)
        {
            _navMeshA.destination = _player.position;
        }
        else
        {
            m_State = MonsterState.Attack;
            _currentTime = attackDelay;
        }
    }

    public void Attack()
    {
        if (Vector3.Distance(_player.position, transform.position) < attackDistance)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > attackDelay)
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
                _currentTime = 0;
            }
        }
        else
        {
            m_State = MonsterState.Move;
            _currentTime = 0;
        }
    }

    public void Damage()
    {
        Debug.Log("우와 이게 뭐야");
    }

    public void HitMonster(int hitPower)
    {
        if (m_State == MonsterState.Damaged || m_State == MonsterState.Die) return;
        if (monsterHp > 0)
        {
            monsterHp -= hitPower;
            m_State = MonsterState.Damaged;
            _isDamaged = true;
            StartCoroutine(WaitDamage());
        }
    }

    IEnumerator WaitDamage()
    {
        yield return new WaitForSeconds(1f);
        m_State = MonsterState.Move;
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
}