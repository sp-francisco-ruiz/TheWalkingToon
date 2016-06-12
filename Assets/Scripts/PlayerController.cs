using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animation))]
public class PlayerController : MonoBehaviour 
{
    enum PlayerState
    {
        Idle,
        Walk,
        Attack,
        Die,
        Damaged
    }

    [SerializeField] float Speed;
    [SerializeField] float RotationSpeed;
    [SerializeField] float RunningSpeed;
    [SerializeField] float AttackPower;
    [SerializeField] float Health;

    [Header("Animations")]
    [SerializeField] string IdleAnimation;
    [SerializeField] string WalkAnimation;
    [SerializeField] List<string> AttackAnimations;
    [SerializeField] string DieAnimation;
    [SerializeField] string DamagedAnimation;

    Rigidbody _rigidBody;
    Animation _animation;
    PlayerState _state;
    List<SlimeController> _enemiesInRange;
    float _currentHealth;

    float _speedMultiplier;
    float SpeedMultiplier
    {
        get
        {
            return _speedMultiplier * (_running ? RunningSpeed : 1f);
        }

        set
        {
            if(_speedMultiplier != value)
            {
                _speedMultiplier = value;
                _animation[WalkAnimation].speed = SpeedMultiplier;
            }
        }
    }

    bool _running;
    bool Running
    {
        get
        {
            return _running;
        }

        set
        {
            if(_running != value)
            {
                _running = value;
                _animation[WalkAnimation].speed = SpeedMultiplier;
            }
        }

    }

    public bool IsAlive
    {
        get
        {
            return _currentHealth > 0f;
        }
    }

	void Awake () 
    {
	    _rigidBody = GetComponent<Rigidbody>();
        _animation = GetComponent<Animation>();
        _enemiesInRange = new List<SlimeController>();
        _currentHealth = Health;
	}

	void Update () 
    {
        if(CanMove())
        {
            if(Input.GetMouseButtonDown(0))
            {
                SetState(PlayerState.Attack);
            }
            else
            {
        	    float hAxis = Input.GetAxis("Horizontal");
                float vaxis = Input.GetAxis("Vertical");

                SpeedMultiplier = vaxis < -0.1f ? -0.5f : 1f;
                Running = Input.GetKey(KeyCode.LeftShift);

                var displacement = transform.forward * Mathf.Abs(vaxis) * Time.deltaTime * Speed * SpeedMultiplier;
                var rotation = new Vector3(0f, hAxis * Time.deltaTime * RotationSpeed * Mathf.Abs(SpeedMultiplier), 0f);

                if(hAxis < -0.1f || hAxis > 0.1f)
                    transform.Rotate(rotation); 

                if(vaxis < -0.1f || vaxis > 0.1f)
                {
                    _rigidBody.MovePosition(_rigidBody.position + displacement);
                    SetState(PlayerState.Walk);
                }
                else
                {
                    SetState(PlayerState.Idle);
                }
            }

            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }
	}

    public bool Hit(float damage)
    {
        bool retVal = false;
       _currentHealth -= damage;
        if(_currentHealth <= 0f)
       {
            _currentHealth = 0f;
            retVal = true;
            SetState(PlayerState.Die);
        }
        Debug.Log("Hero Damaged for: " + damage + " resulting " + _currentHealth + " health");
       return retVal;
    }

    bool CanMove()
    {
        return _state == PlayerState.Walk || _state == PlayerState.Idle;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && !other.isTrigger)
        {
            var target = other.GetComponent<SlimeController>();
            if(target != null)
                _enemiesInRange.Add(target);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy") && !other.isTrigger)
        {
            var target = other.GetComponent<SlimeController>();
            if(target != null)
                _enemiesInRange.Remove(target);
        }
    }

    void AttackDamage()
    {
        if(_enemiesInRange.Count > 0)
        {
            var closestEnemy = _enemiesInRange[0];
            for(int i = 0; i < _enemiesInRange.Count; ++i)
            {
                var enemy = _enemiesInRange[i];
                if((enemy.transform.position - transform.position).sqrMagnitude < (closestEnemy.transform.position - transform.position).sqrMagnitude)
                    closestEnemy = enemy;
            }
            if(closestEnemy.Hit(AttackPower))
                _enemiesInRange.Remove(closestEnemy);
        }
    }

    void AttackEnd()
    {
        SetState(PlayerState.Idle);
    }

    void SetState(PlayerState state)
    {
        if(_state != state)
        {
            _state = state;
            switch(_state)
            {
                case PlayerState.Walk:
                    _animation.CrossFade(WalkAnimation);
                break;
                case PlayerState.Idle:
                    _animation.CrossFade(IdleAnimation);
                break;
                case PlayerState.Attack:
                    _animation.CrossFade(AttackAnimations[Random.Range(0, AttackAnimations.Count)]);
                break;
                case PlayerState.Die:
                    _animation.CrossFade(DieAnimation);
                break;
            }
        }
    }
}
