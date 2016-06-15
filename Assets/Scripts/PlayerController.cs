using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animation))]
public class PlayerController : MonoBehaviour 
{
    enum PlayerState
    {
        Idle,
        Walk,
        Chase,
        Attack,
        Die,
        Damaged
    }

    [SerializeField] GameObject GroundGameObject;

    [Header("Stats")]
    [SerializeField] float Speed;
    [SerializeField] float RotationSpeed;
    [SerializeField] float RunningSpeed;
    [SerializeField] float AttackPower;
    [SerializeField] float Health;
    [SerializeField] float AttackRange;

    [Header("Animations")]
    [SerializeField] string IdleAnimation;
    [SerializeField] string WalkAnimation;
    [SerializeField] List<string> AttackAnimations;
    [SerializeField] string DieAnimation;
    [SerializeField] string DamagedAnimation;

    [Header("UI")]
    [SerializeField] Image HealthBar;
    [SerializeField] Text MonsterCountLabel;

    [Header("Particles")]
    [SerializeField] GameObject HealingEffect;

    Rigidbody _rigidBody;
    Animation _animation;
    PlayerState _state;
    List<MonsterController> _enemiesInRange;
    Coroutine _healCoroutine;
    float _currentHealth;
	float _currentAnimationTime;
    int _enemiesKilled;

    MonsterController _target;

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

    Vector3 _targetPos;
    void OnClickedAsRightButtonEvent(Game.Managers.ClickedAsRightButtonEvent e)
    {
        if(e.TouchedObject == GroundGameObject)
        {
            if(CanMove())
            {
                SetState(PlayerState.Walk);
                _targetPos = e.TouchedPosition;
                _agent.SetDestination(_targetPos);
            }
        }
        else
        {

        }
    }
    NavMeshAgent _agent;
	void Awake () 
    {
	    _rigidBody = GetComponent<Rigidbody>();
        _animation = GetComponent<Animation>();
        _agent = GetComponent<NavMeshAgent>();
        _enemiesInRange = new List<MonsterController>();
        _currentHealth = Health;
        HealingEffect.SetActive(false);
        _enemiesKilled = 0;

        _agent.speed = Speed;

        Game.Managers.EventDispatcher.Instance.AddListener<Game.Managers.ClickedAsRightButtonEvent>(OnClickedAsRightButtonEvent);
	}


	void Update () 
    {
        Game.Managers.InputManager.Instance.Update();
        if(_state == PlayerState.Chase)
        {
            if(_target == null || !_target.IsAlive)
                SetState(PlayerState.Idle);
            else if( (transform.position - _target.transform.position).sqrMagnitude <= AttackRange * AttackRange)
                SetState(PlayerState.Attack);
            else
                _agent.SetDestination(_target.transform.position);
        }
        else if (/*CanMove ()*/_state == PlayerState.Walk)
        {
            if((_targetPos - transform.position).sqrMagnitude < 0.5f)
                SetState(PlayerState.Idle);
            return;
			if (Input.GetMouseButtonDown (0)) 
            {
				SetState (PlayerState.Attack);
			} 
            else 
            {
				float hAxis = Input.GetAxis ("Horizontal");
				float vaxis = Input.GetAxis ("Vertical");

				SpeedMultiplier = vaxis < -0.1f ? -0.5f : 1f;
				Running = Input.GetKey (KeyCode.LeftShift);

				var displacement = transform.forward * Mathf.Abs (vaxis) * Time.deltaTime * Speed * SpeedMultiplier;
				var rotation = new Vector3 (0f, hAxis * Time.deltaTime * RotationSpeed * Mathf.Abs (SpeedMultiplier), 0f);

				if (hAxis < -0.1f || hAxis > 0.1f)
					transform.Rotate (rotation); 

				if (vaxis < -0.1f || vaxis > 0.1f) 
                {
					_rigidBody.MovePosition (_rigidBody.position + displacement);
					SetState (PlayerState.Walk);
				} 
                else
                {
					SetState (PlayerState.Idle);
				}
			}

			_rigidBody.velocity = Vector3.zero;
			_rigidBody.angularVelocity = Vector3.zero;
		}
		else if (_state == PlayerState.Attack)
		{
			if (_currentAnimationTime == _animation [AttackAnimations [0]].normalizedTime)
			{
				if (_state != PlayerState.Die)
					SetState (PlayerState.Idle);
			}
			else
				_currentAnimationTime = _animation [AttackAnimations [0]].normalizedTime;	
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
        HealthBar.fillAmount = _currentHealth / Health;
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
            var target = other.GetComponent<MonsterController>();
            if(target != null)
                _enemiesInRange.Add(target);
        }
        else if(other.CompareTag("Pickup"))
        {
            Destroy(other.gameObject, 0.2f);
            var pickup = other.GetComponent<HealPickup>();
            if(pickup != null)
            {
                if(_healCoroutine != null)
                    StopCoroutine(_healCoroutine);
                _healCoroutine = StartCoroutine(HealOverTime(pickup.HealPerSecond, pickup.Duration));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy") && !other.isTrigger)
        {
            var target = other.GetComponent<MonsterController>();
            if(target != null)
                _enemiesInRange.Remove(target);
        }
    }

    IEnumerator HealOverTime(float hps, float duration)
    {
        HealingEffect.SetActive(true);
        float remainingtime = duration;
        do
        {
            _currentHealth += hps * Time.deltaTime;
            _currentHealth = Mathf.Min(Health, _currentHealth);
            remainingtime -= Time.deltaTime;
            HealthBar.fillAmount = _currentHealth / Health;
            yield return null;
        }
        while(remainingtime > 0f && _state != PlayerState.Die);
        HealingEffect.SetActive(false);
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
            {
                _enemiesInRange.Remove(closestEnemy);
                ++_enemiesKilled;
                MonsterCountLabel.text = "Monsters killed: " + _enemiesKilled;
            }
        }
    }

    void AttackEnd()
    {
//		if(_state != PlayerState.Die)
//            SetState(PlayerState.Idle);
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
					_currentAnimationTime = -1f;
                    _animation.CrossFade(AttackAnimations[Random.Range(0, AttackAnimations.Count)]);
                break;
                case PlayerState.Die:
                    _animation.CrossFade(DieAnimation);
                break;
            }
        }
    }
}
