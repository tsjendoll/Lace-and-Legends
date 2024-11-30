using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerGroundedState
{
    public PlayerWalkState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput * player.walkSpeed, rb.velocity.y);


        if (xInput == 0)
            stateMachine.ChangeState(player.idleState);
        else 
        {
            if (Input.GetKey(KeyCode.LeftShift))
                stateMachine.ChangeState(player.runState);
        }
        if (!player.IsGroundDetected() && !player.IsSolidDetected())
            stateMachine.ChangeState(player.airState);

    }

    
}
