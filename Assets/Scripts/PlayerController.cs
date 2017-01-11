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
        Attack,
        Die,
        Damaged
    }
    [Header("Stats")]
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

    float _speedMultiplier;
    float SpeedMultiplier
    {
        get
        {
            return _speedMultiplier * (_running ? RunningSpeed : 1f);
        }

        set
        {
            if(System.Math.Abs(_speedMultiplier - value) > float.Epsilon)
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
        _enemiesInRange = new List<MonsterController>();
        _currentHealth = Health;
        HealingEffect.SetActive(false);
        _enemiesKilled = 0;
	}

    bool _rotating = false;
    Vector3 _lastMousePos = Vector3.zero;
	void Update () 
    {
        if(Input.GetMouseButtonDown(1))
        {
            _rotating = true;
            _lastMousePos = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            _rotating = false;
        }
        if (CanMove ()) 
        {
            if (Input.GetMouseButtonDown (0)) 
            {
                SetState (PlayerState.Attack);
            } 
            else 
            {
				float hAxis = Input.GetAxis ("Horizontal");
                float vAxis = Input.GetAxis ("Vertical");

				SpeedMultiplier = vAxis < -0.1f ? -0.5f : 1f;
				Running = Input.GetKey (KeyCode.LeftShift);

                var displacement = Vector3.zero;
                var rotation = Vector3.zero;

                if(vAxis < -0.1f || vAxis > 0.1f)
                {
                    displacement += transform.forward * Mathf.Abs (vAxis) * Time.deltaTime * Speed * SpeedMultiplier;
                }

                if(_rotating)
                {
                    if (hAxis < -0.1f || hAxis > 0.1f)
                    {
                        var localDisplacement = transform.right * hAxis * Time.deltaTime * Speed * SpeedMultiplier;
                        if(SpeedMultiplier < 0.0f)
                        {
                            localDisplacement *= -1.0f;
                        }
                        displacement += localDisplacement;
                    }
                    float amount = Input.mousePosition.x - _lastMousePos.x;
                    hAxis = Mathf.Min(Mathf.Max(-1.5f, amount), 1.5f);
                    _lastMousePos = Input.mousePosition;
                }

				if (hAxis < -0.1f || hAxis > 0.1f)
                {
                    rotation += new Vector3 (0f, hAxis * Time.deltaTime * RotationSpeed * Mathf.Abs (SpeedMultiplier), 0f);
                }

                if(rotation.sqrMagnitude > 0.0f)
                {
                    transform.Rotate (rotation); 
                }
                if (displacement.sqrMagnitude > 0.0f) 
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
            if(System.Math.Abs(_currentAnimationTime - _animation[AttackAnimations[0]].normalizedTime) < float.Epsilon)
            {
                if(_state != PlayerState.Die)
                    SetState(PlayerState.Idle);
            }
            else
                _currentAnimationTime = _animation[AttackAnimations[0]].normalizedTime;	
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
