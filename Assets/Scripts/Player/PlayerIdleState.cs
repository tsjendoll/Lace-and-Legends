using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        if (player.lastAnimBoolName == "Run")
        {
            stateTimer = .1f;
            player.SetVelocity(player.attackMovement.x * player.facingDir, player.attackMovement.y);
        }
        else if (player.lastAnimBoolName == "Walk")
        {
            stateTimer = .05f;
            player.SetVelocity(player.attackMovement.x / 2* player.facingDir, player.attackMovement.y / 2);
        }
        else
            stateTimer = 0f;
    }

    public override void Update()
    {
        

        if (stateTimer < 0 && xInput == 0 && player.IsGroundDetected())
            player.setZeroVelocity();

        if (xInput != 0 && Input.GetKey(KeyCode.LeftShift))
                stateMachine.ChangeState(player.runState);
        else if (xInput != 0)
                stateMachine.ChangeState(player.walkState);

            base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
