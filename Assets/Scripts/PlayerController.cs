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

    [Header("Animations")]
    [SerializeField] string IdleAnimation;
    [SerializeField] string WalkAnimation;
    [SerializeField] List<string> AttackAnimations;
    [SerializeField] string DieAnimation;
    [SerializeField] string DamagedAnimation;

    Rigidbody _rigidBody;
    Animation _animation;
    PlayerState _state;

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

	void Awake () 
    {
	    _rigidBody = GetComponent<Rigidbody>();
        _animation = GetComponent<Animation>();
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

                SpeedMultiplier = vaxis < 0.1f ? -0.5f : 1f;
                Running = Input.GetKey(KeyCode.LeftShift);

                if(vaxis > 0.1f || vaxis < -0.1f)
                {
                    var displacement = transform.forward * Mathf.Abs(vaxis) * Time.deltaTime * Speed * SpeedMultiplier;

                    _rigidBody.MovePosition(_rigidBody.position + displacement);

                    SetState(PlayerState.Walk);
                }
                else
                {
                    SetState(PlayerState.Idle);
                }

                if(hAxis < -0.1f || hAxis > 0.1f)
                {
                    var rotation = new Vector3(0f, hAxis * Time.deltaTime * RotationSpeed * Mathf.Abs(SpeedMultiplier), 0f);
                    transform.Rotate(rotation); 
                }
            }

            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }
	}

    bool CanMove()
    {
        return _state == PlayerState.Walk || _state == PlayerState.Idle;
    }

    void AttackDamage()
    {

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

            }
        }
    }
}
