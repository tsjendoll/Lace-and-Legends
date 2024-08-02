using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move Info")]
    public float walkSpeed = 12f;
    public float runSpeed = 20f;

    #region Components
    public List<Animator> anims { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Animator playerAnimator {get; private set; }
    public Animator clothingAnimator {get; private set; }
    #endregion

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerWalkState walkState { get; private set; }
    public PlayerRunState runState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }

    #endregion

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        walkState = new PlayerWalkState(this, stateMachine, "Walk");
        runState = new PlayerRunState(this, stateMachine, "Run");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
    }

    public void Start()
    {
        anims = new List<Animator>(GetComponentsInChildren<Animator>());
        rb = GetComponent<Rigidbody2D>();
        stateMachine.Initialize(idleState);
        GameObject playerObj = GameObject.Find("Animator");
        GameObject clothingObj = GameObject.Find("Clothing");
        clothingAnimator = clothingObj.GetComponent<Animator>();
        playerAnimator = playerObj.GetComponent<Animator>();

    }

    private void Update()
    {
        stateMachine.currentState.Update();
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
    }
}
