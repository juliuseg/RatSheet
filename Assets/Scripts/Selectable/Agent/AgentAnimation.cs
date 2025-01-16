using UnityEngine;

public class AgentAnimation : MonoBehaviour
{
    [SerializeField]private Animator _anim;

    [SerializeField] private float _attackAnimTime = 0.2f;


    private float _lockedTill;

    private Vector2 direction;


    public void SetState (AttackState attackState, Vector2 velocity, SpriteRenderer sr) {        
        // locked
        if (Time.time < _lockedTill) {
            //print("locked");
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


