using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : Entity
{

    [Header("ATtack Details")]
    public Vector2 attackMovement;
    
    [Header("Move Info")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public bool coyoteTime = true;
    public float coyoteTimeDuration;
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
}
