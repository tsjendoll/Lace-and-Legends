using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        stateTimer = player.coyoteTimeDuration;
    }

    public override void Exit()
    {
        base.Exit();
        player.coyoteTime = true;
    }

    public override void Update()
    {
        base.Update();
        
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
