using UnityEngine;
using System.Collections;

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

    [Header("Animations")]
    [SerializeField] string IdleAnimation;
    [SerializeField] string WalkAnimation;
    [SerializeField] string AttackAnimation;
    [SerializeField] string DieAnimation;
    [SerializeField] string DamagedAnimation;

    Rigidbody _rigidBody;
    Animation _animation;
    PlayerState _state;

	void Awake () 
    {
	    _rigidBody = GetComponent<Rigidbody>();
        _animation = GetComponent<Animation>();
	}

	void Update () 
    {
	    float hAxis = Input.GetAxis("Horizontal");
        float vaxis = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(hAxis, 0, vaxis).normalized;

        if(direction.sqrMagnitude > 0)
        {
            transform.LookAt(transform.position + direction);
            SetState(PlayerState.Walk);
        }
        else
        {
            SetState(PlayerState.Idle);
        }

        _rigidBody.MovePosition(_rigidBody.position + direction * Speed * Time.deltaTime);
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
            }
        }
    }
}
