using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        player.coyoteTime = false;
        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();    
        if (rb.velocity.y < 0)
            stateMachine.ChangeState(player.airState);

        if (player.doubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            player.doubleJump = false;
            stateMachine.ChangeState(player.jumpState);
        }
        
        if(xInput != 0 && Input.GetKey(KeyCode.LeftShift))
            player.SetVelocity(xInput * player.runSpeed * .8f, rb.velocity.y);
        else if(xInput != 0)
            player.SetVelocity(xInput * player.walkSpeed * .8f, rb.velocity.y);
    }
}
