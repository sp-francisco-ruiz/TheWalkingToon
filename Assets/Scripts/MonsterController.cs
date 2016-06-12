using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animation))]
public class MonsterController : MonoBehaviour 
{
    enum MonsterState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Die,
        Damaged
    }

    [SerializeField] string IdleAnimation;
    [SerializeField] string WalkAnimation;
    [SerializeField] string AttackAnimation;
    [SerializeField] string DamagedAnimation;
    [SerializeField] string DieAnimation;

    [Header("Creature Info")]
    [SerializeField] MonsterInfo Info;

    [Header("Movement Info")]
    [SerializeField] float SpawnTime;
    [SerializeField] float MinIdleTime;
    [SerializeField] float MaxIdleTime;
    [SerializeField] Transform SpawnPoint;
    [SerializeField] List<Transform> PatrolPoints;

    [Header("UI")]
    [SerializeField] Image HealthBar;
    [SerializeField] GameObject UIRoot;

    NavMeshAgent _agent;
    Animation _animation;
    Collider _collider;
    MonsterState _state;
    PlayerController _target;
    int _targetPatrolPoint;
    float _health;
    float _timeWaiting;
    float _timeTowait;

    float _lastAttackTime;

    public bool IsAlive
    {
        get
        {
            return _health > 0f;
        }
    }

	void Awake () 
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = Info.Speed;
        _animation = GetComponent<Animation>();
        _collider = GetComponent<Collider>();
	    _health = Info.Health;
        float animSpeedFactor = 1f / _animation[AttackAnimation].length;
        _animation[AttackAnimation].speed = Info.AttacksPerSecond / animSpeedFactor;
        UIRoot.SetActive(false);
	}

	void Update () 
    {

        if(_state == MonsterState.Chase)
        {
            if(_target == null || !_target.IsAlive)
                SetState(MonsterState.Patrol);
            else if( (transform.position - _target.transform.position).sqrMagnitude <= Info.AttackRange * Info.AttackRange)
                SetState(MonsterState.Attack);
            else
                _agent.SetDestination(_target.transform.position);
        }
        else if(_state == MonsterState.Patrol)
        {
            if(_target != null && _target.IsAlive)
                SetState(MonsterState.Chase);
            else if((transform.position - PatrolPoints[_targetPatrolPoint].position).sqrMagnitude < 0.1f)
                SetState(MonsterState.Idle);
        }
        else if(_state == MonsterState.Idle)
        {
            if(_target != null && _target.IsAlive && (transform.position - _target.transform.position).sqrMagnitude <= Info.AttackRange * Info.AttackRange)
                SetState(MonsterState.Attack);
            else if (_target != null && _target.IsAlive)
                SetState(MonsterState.Chase);
            else if((_timeWaiting += Time.deltaTime) > _timeTowait)
                SetState(MonsterState.Patrol);
        }
        else if(_state == MonsterState.Attack)
        {
            if(_target != null)
                transform.LookAt(_target.transform);

            if(_lastAttackTime == _animation[AttackAnimation].normalizedTime)
                SetState(MonsterState.Idle);
            else
                _lastAttackTime = _animation[AttackAnimation].normalizedTime;
        }
        else if(_state == MonsterState.Die)
        {
            if((_timeTowait -= Time.deltaTime) <= 0f)
            {
                Respawn();
            }
        }
	}

    void Respawn()
    {
        HealthBar.fillAmount = 1f;
        _health = Info.Health;
        transform.position = SpawnPoint.position;
        _agent.enabled = true;
        if(_collider != null)
            _collider.enabled = true;
        SetState(MonsterState.Idle);
    }

    void OnAttackDoesDamage()
    {
        if(_target != null && _target.Hit(Info.Damage))
            _target = null;
    }

    void OnAttackEnds()
    {
        //if(_state != MonsterState.Die)
        //    SetState(MonsterState.Idle);
    }

    public bool Hit(float damage)
    {
        bool retVal = false;
        _health -= damage;
        if(_health <= 0f)
        {
            _health = 0f;
            retVal = true;
            SetState(MonsterState.Die);
        }
        else
        {
            if(!UIRoot.activeInHierarchy)
                UIRoot.SetActive(true);
            HealthBar.fillAmount = _health / Info.Health;
        }
        return retVal;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            _target = other.GetComponent<PlayerController>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _target = null;
        }
    }

    void SetState(MonsterState state)
    {
        if(_state != state)
        {
            _state = state;
            switch(state)
            {
                case MonsterState.Patrol:
                    _agent.Resume();
                    _targetPatrolPoint = Random.Range(0, PatrolPoints.Count);
                    _animation.CrossFade(WalkAnimation);
                    _agent.SetDestination(PatrolPoints[_targetPatrolPoint].position);
                break;

                case MonsterState.Chase:
                    _agent.Resume();
                    _animation.CrossFade(WalkAnimation);
                break;

                case MonsterState.Attack:
                    _lastAttackTime = -1f;
                    if(_agent.isActiveAndEnabled)
                        _agent.Stop();
                    _animation.CrossFade(AttackAnimation);
                break;

                case MonsterState.Idle:
                    _timeTowait = Random.Range(MinIdleTime, MaxIdleTime);
                    _timeWaiting = 0f;
                    _animation.CrossFade(IdleAnimation);
                break;

                case MonsterState.Die:
                    UIRoot.SetActive(false);
                    _agent.enabled = false;
                    if(_collider != null)
                        _collider.enabled = false;
                    _animation.CrossFade(DieAnimation);
                    _timeTowait = SpawnTime;
                break;
            }
        }
    }
}
