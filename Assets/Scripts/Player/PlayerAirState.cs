using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{

    private float gravityLerpTime = 0f;
    private float gravityLerpDuration = 2f; // Adjust to control how quickly the lerp completes

    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        stateTimer = player.coyoteTimeDuration;

        if (player.floatWithSkirt)
        {
            gravityLerpTime = 0f; // Reset lerp time
            player.rb.gravityScale = 0f; // Start from 0 gravity  
        }
        else
            player.rb.gravityScale = 5f;
    }

    public override void Exit()
    {
        base.Exit();
        player.coyoteTime = true;
        player.rb.gravityScale = 3.5f;
    }

    public override void Update()
    {
        base.Update();

        if (player.rb.gravityScale < 3.5f && player.floatWithSkirt)
        {
            gravityLerpTime += Time.deltaTime; // Increment lerp time
            player.rb.gravityScale = Mathf.Lerp(0f, 3.5f, gravityLerpTime / gravityLerpDuration);
        }

        // Prevent overshooting the target value
        if (player.rb.gravityScale > 3.5f && player.floatWithSkirt)
        {
            player.rb.gravityScale = 3.5f;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            player.rb.gravityScale = 3.5f;
        }
        
        if (Input.GetKey(KeyCode.Space) && stateTimer > 0 && player.coyoteTime == true) {
            stateMachine.ChangeState(player.jumpState);
        }

        if (player.jumpCount < 2 && Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.jumpState);
        }
           
        if (player.IsGroundDetected() || player.IsSolidDetected())
            stateMachine.ChangeState(player.idleState);

        if(xInput != 0 && Input.GetKey(KeyCode.LeftShift))
            player.SetVelocity(xInput * player.runSpeed * .8f, rb.velocity.y);
        else if(xInput != 0)
            player.SetVelocity(xInput * player.walkSpeed * .8f, rb.velocity.y);
    }
}
