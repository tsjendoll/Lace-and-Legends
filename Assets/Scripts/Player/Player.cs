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
    public bool floatWithSkirt;

    [HideInInspector] public int jumpCount = 0;
    [HideInInspector] public int airDashCount = 0;

    [Header("Dash Info")]
    public float dashDuration;
    public float dashSpeed;
    public float dashDir { get; private set; }

    [Header("Clothing Info")]
    public bool IsCorsetActive = true;

    public GameObject sword { get; private set; }

    #region Components
    public SkillManager skill { get; private set; }
    public Animator playerAnimator {get; private set; }
    public Animator topAnimator {get; private set; }
    public Animator bottomAnimator {get; private set; }
    public Animator hairAnimator {get; private set; }
    #endregion

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerWalkState walkState { get; private set; }
    public PlayerRunState runState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerAttackState attackState {get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    #endregion

    [SerializeField] private CameraFollowObject cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        walkState = new PlayerWalkState(this, stateMachine, "Walk");
        runState = new PlayerRunState(this, stateMachine, "Run");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Run");

        attackState = new PlayerAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        skill = SkillManager.instance;
        
        playerAnimator = PlayerManager.instance.playerAnimator;
        topAnimator = PlayerManager.instance.topAnimator;
        bottomAnimator = PlayerManager.instance.bottomAnimator;
        hairAnimator = PlayerManager.instance.hairAnimator;

        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
        
        CheckForDashInput();

        //if we are falling past a certain speed threshold and lerping is not currently already running
        if (rb.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }
        //if we are standing still or moving up
        if (rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            // reset so is can be called again
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
        }
    }

    public void AssignNewSword(GameObject _newSword)
    {
        if (sword != null)
            Destroy(sword);
        sword = _newSword;
    }
        
    public void CatchSword()
    {        
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    private void CheckForDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && SkillManager.instance.dash.CanUseSkill())
        {
            // only allow to dash in air once
            if (!IsGroundDetected() && airDashCount >= 1)
                return;

            dashDir = Input.GetAxisRaw("Horizontal");

            if(dashDir == 0)
                dashDir = facingDir;
            if (!IsGroundDetected())
                airDashCount += 1;
            stateMachine.ChangeState(dashState);
        }
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public override void Flip()
    {
        base.Flip();
        cameraFollowObject.CallTurn();
    }


}
