using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : Entity
{

    [Header("Attack Details")]
    public Vector2 attackMovement;
    public float CounterAttackDuration = .2f;

    
    [Header("Move Info")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public bool coyoteTime = true;
    public float coyoteTimeDuration;
    [HideInInspector]
    public bool doubleJump = true;

    #region Components
    
    public Animator playerAnimator {get; private set; }
    public Animator topAnimator {get; private set; }
    public Animator bottomAnimator {get; private set; }

    #endregion

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerWalkState walkState { get; private set; }
    public PlayerRunState runState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    
    public PlayerAttackState attackState {get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        walkState = new PlayerWalkState(this, stateMachine, "Walk");
        runState = new PlayerRunState(this, stateMachine, "Run");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        attackState = new PlayerAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
        playerAnimator = GameObject.Find("PlayerAnimator").GetComponent<Animator>();
        topAnimator = GameObject.Find("Top").GetComponent<Animator>();
        bottomAnimator = GameObject.Find("Bottom").GetComponent<Animator>();

    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    /// <summary>
    /// Returns an array of colliders within a specified circle, filtered by the player's facing direction.
    /// </summary>
    /// <param name="position">The center position of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="facingDir">
    /// The direction the player is facing:
    /// <list type="bullet">
    /// <item><description>1 for right</description></item>
    /// <item><description>-1 for left</description></item>
    /// </list>
    /// </param>
    /// <returns>An array of colliders within the specified half of the circle based on the facing direction.</returns>
    public Collider2D[] GetFilteredColliders(Vector2 position, float radius, int facingDir)
    {
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(position, radius);
        List<Collider2D> filteredColliders = new List<Collider2D>();

        foreach (Collider2D collider in allColliders)
        {
            float relativeX = collider.transform.position.x - position.x;

            // Filter based on the facing direction
            if ((facingDir == -1 && relativeX <= 0) ||  // Left half if facing left
                (facingDir == 1 && relativeX >= 0))     // Right half if facing right
            {
                filteredColliders.Add(collider);
            }
        }

        return filteredColliders.ToArray();
    }
}
