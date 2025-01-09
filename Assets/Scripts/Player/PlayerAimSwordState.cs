using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = .2f;

        player.skill.sword.DotsActive(true);
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            player.setZeroVelocity();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);

        if ((Helpers.Camera.ScreenToWorldPoint(Input.mousePosition).x < player.transform.position.x && player.facingDir != -1) ||
                (Helpers.Camera.ScreenToWorldPoint(Input.mousePosition).x > player.transform.position.x && player.facingDir != 1 ))
                    player.Flip();
    }
}
