using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;

    public PlayerCatchSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = .2f;
        sword = player.sword.transform;

        if ((sword.position.x < player.transform.position.x && player.facingDir != -1) ||
                (sword.position.x > player.transform.position.x && player.facingDir != 1 ))
                    player.Flip();
        
        rb.velocity = new Vector2(5 * -player.facingDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer <= 0)
            player.stateMachine.ChangeState(player.idleState);
    }
    
}
