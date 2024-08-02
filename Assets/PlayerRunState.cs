using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput * player.runSpeed, rb.velocity.y);

        if (xInput == 0)
            stateMachine.ChangeState(player.idleState);
        else 
        {
            if (Input.GetKeyUp(KeyCode.LeftShift))
                stateMachine.ChangeState(player.walkState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
