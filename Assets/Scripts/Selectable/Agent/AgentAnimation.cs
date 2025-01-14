using UnityEngine;

public class AgentAnimation : MonoBehaviour
{
    [SerializeField]private Animator _anim;

    [SerializeField] private float _attackAnimTime = 0.2f;


    private float _lockedTill;

    private Vector2 direction;

    private void Start() {
        // Debug.Log($"IdleDown: {IdleDown}");
        // Debug.Log($"IdleUp: {IdleUp}");
        // Debug.Log($"AttackDown: {AttackDown}");
        // Debug.Log($"AttackUp: {AttackUp}");
        // Debug.Log($"WalkDown: {WalkDown}");
        // Debug.Log($"WalkUp: {WalkUp}");
    }


    public void SetState (AttackState attackState, Vector2 velocity, SpriteRenderer sr) {
        print (attackState);
        
        // locked
        if (Time.time < _lockedTill) {
            print("locked");
            return;
        }

        var state = Idle;
        if (attackState == AttackState.moving || attackState == AttackState.movingToAttack
        || (attackState == AttackState.reloading && velocity != Vector2.zero)){
            // We are moving
            // Set animation to walking
            state = direction.y > 0 ? WalkUp : WalkDown;
            // Save direction

            if (velocity != Vector2.zero) // This should be true always, but just in case
                direction = velocity;
            

        }

        if (attackState == AttackState.reloading && velocity == Vector2.zero) {
            state = direction.y > 0 ? AttackIdleUp : AttackIdleDown;
        }

        if (attackState == AttackState.attacking){
            // We are not moving
            // We need to attack
            var aState  = direction.y > 0 ? AttackUp : AttackDown;
            state = LockState(aState, _attackAnimTime);
            // We use the same direction for the attack as the saved direction
        }

        if (attackState == AttackState.idle){
            // We are idle so we use same direction as saved direction.
            state = direction.y > 0 ? IdleUp : IdleDown;
        }

        // Flip sprite!
        if (velocity != Vector2.zero){
            sr.flipX = velocity.x < 0f;
        }
        
        if (state == _currentState) return;
        _anim.CrossFade(state, 0, 0);
        _currentState = state;


        

        int LockState(int s, float t) {
            _lockedTill = Time.time + t;
            return s;
        }

        
    }


    #region Cached Properties

    private int _currentState;
    private int _currentDirection;
    private static readonly int IdleDown = Animator.StringToHash("IdleDown");
    private static readonly int IdleUp = Animator.StringToHash("IdleUp");
    private static readonly int AttackDown = Animator.StringToHash("AttackDown");
    private static readonly int AttackUp = Animator.StringToHash("AttackUp");
    private static readonly int WalkDown = Animator.StringToHash("WalkDown");
    private static readonly int WalkUp = Animator.StringToHash("WalkUp");

    private static readonly int AttackIdleDown = Animator.StringToHash("AttackIdleDown");
    private static readonly int AttackIdleUp = Animator.StringToHash("AttackIdleUp");


    private static readonly int Idle = 0;
    private static readonly int Attack = 1;
    private static readonly int Walk = 2;

    private static readonly int Down = 3;
    private static readonly int Up = 4;

    


    #endregion
}



/*
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PlayerAnimator : MonoBehaviour {
    [SerializeField] private float _minImpactForce = 20;
    
    // Anim times can be gathered from the state itself, but 
    // for the simplicity of the video...
    [SerializeField] private float _landAnimDuration = 0.1f;
    [SerializeField] private float _attackAnimTime = 0.2f;

    private IPlayerController _player;
    private Animator _anim;
    private SpriteRenderer _renderer;
    
    private bool _grounded;
    private float _lockedTill;
    private bool _jumpTriggered;
    private bool _attacked;
    private bool _landed;

    private void Awake() {
        if (!TryGetComponent(out IPlayerController player)) {
            Destroy(this);
            return;
        }

        _player = player;
        _anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }


    private void Start() {
        _player.Jumped += () => {
            _jumpTriggered = true;
        };
        _player.Attacked += () => {
            _attacked = true;
        };
        _player.GroundedChanged += (grounded, impactForce) => {
            _grounded = grounded;
            _landed = impactForce >= _minImpactForce;
        };
    }

    private void Update() {
        if (_player.Input.x != 0) _renderer.flipX = _player.Input.x < 0;

        var state = GetState();

        _jumpTriggered = false;
        _landed = false;
        _attacked = false;

        if (state == _currentState) return;
        _anim.CrossFade(state, 0, 0);
        _currentState = state;
    }

    private int GetState() {
        if (Time.time < _lockedTill) return _currentState;

        // Priorities
        if (_attacked) return LockState(Attack, _attackAnimTime);
        if (_player.Crouching) return Crouch;
        if (_landed) return LockState(Land, _landAnimDuration);
        if (_jumpTriggered) return Jump;

        if (_grounded) return _player.Input.x == 0 ? Idle : Walk;
        return _player.Speed.y > 0 ? Jump : Fall;

        int LockState(int s, float t) {
            _lockedTill = Time.time + t;
            return s;
        }
    }

    #region Cached Properties

    private int _currentState;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private static readonly int Land = Animator.StringToHash("Land");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Crouch = Animator.StringToHash("Crouch");

    #endregion
}

public interface IPlayerController {
    public Vector2 Input { get; }
    public Vector2 Speed { get; }
    public bool Crouching { get; }

    public event Action<bool, float> GroundedChanged; // Grounded - Impact force
    public event Action Jumped;
    public event Action Attacked;
}

*/