using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animation))]
public class SlimeController : MonoBehaviour 
{
    enum SlimeState
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

    NavMeshAgent _agent;
    Animation _animation;
    Collider _collider;
    SlimeState _state;
    PlayerController _target;
    int _targetPatrolPoint;
    float _health;
    float _timeWaiting;
    float _timeTowait;

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
	}

	void Update () 
    {

        if(_state == SlimeState.Chase)
        {
            if(_target == null || !_target.IsAlive)
                SetState(SlimeState.Patrol);
            else if( (transform.position - _target.transform.position).sqrMagnitude <= Info.AttackRange * Info.AttackRange)
                SetState(SlimeState.Attack);
            else
                _agent.SetDestination(_target.transform.position);
        }
        else if(_state == SlimeState.Patrol)
        {
            if(_target != null)
                SetState(SlimeState.Chase);
            else if((transform.position - PatrolPoints[_targetPatrolPoint].position).sqrMagnitude < 0.1f)
                SetState(SlimeState.Idle);
        }
        else if(_state == SlimeState.Idle)
        {
            if(_target != null && _target.IsAlive && (transform.position - _target.transform.position).sqrMagnitude <= Info.AttackRange * Info.AttackRange)
                SetState(SlimeState.Attack);
            else if (_target != null && _target.IsAlive)
                SetState(SlimeState.Chase);
            else if((_timeWaiting += Time.deltaTime) > _timeTowait)
                SetState(SlimeState.Patrol);
        }
        else if(_state == SlimeState.Attack)
        {
            if(_target != null)
                transform.LookAt(_target.transform);
        }
        else if(_state == SlimeState.Die)
        {
            if((_timeTowait -= Time.deltaTime) <= 0f)
            {
                Respawn();
            }
        }
	}

    void Respawn()
    {
        _health = Info.Health;
        transform.position = SpawnPoint.position;
        _agent.enabled = true;
        if(_collider != null)
            _collider.enabled = true;
        SetState(SlimeState.Idle);
    }

    void OnAttackDoesDamage()
    {
        if(_target != null && _target.Hit(Info.Damage))
            _target = null;
    }

    void OnAttackEnds()
    {
        if(_state != SlimeState.Die)
            SetState(SlimeState.Idle);
    }

    public bool Hit(float damage)
    {
        bool retVal = false;
       _health -= damage;
       if(_health <= 0f)
       {
            _health = 0f;
            retVal = true;
            SetState(SlimeState.Die);
       }
       Debug.Log("Slime hit for " + damage + " resulting " + _health + " health");
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

    void SetState(SlimeState state)
    {
        if(_state != state)
        {
            Debug.Log("current: " + _state + " new " + state);
            _state = state;
            switch(state)
            {
                case SlimeState.Patrol:
                    _agent.Resume();
                    _targetPatrolPoint = Random.Range(0, PatrolPoints.Count);
                    _animation.CrossFade(WalkAnimation);
                    _agent.SetDestination(PatrolPoints[_targetPatrolPoint].position);
                break;

                case SlimeState.Chase:
                    _agent.Resume();
                    _animation.CrossFade(WalkAnimation);
                break;

                case SlimeState.Attack:
                    if(_agent.isActiveAndEnabled)
                        _agent.Stop();
                    _animation.CrossFade(AttackAnimation);
                break;

                case SlimeState.Idle:
                    _timeTowait = Random.Range(MinIdleTime, MaxIdleTime);
                    _timeWaiting = 0f;
                    _animation.CrossFade(IdleAnimation);
                break;

                case SlimeState.Die:
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
