using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Transform player;
    private EnemySkeleton enemy;
    private int moveDir;

     // Get the layer indices
    int enemyLayer = LayerMask.NameToLayer("Enemy");
    int boundaryLayer = LayerMask.NameToLayer("Boundary");

    
    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, EnemySkeleton enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = GameObject.Find("Player").transform;

        // Disable collision between Enemy and Boundary layers
        Physics2D.IgnoreLayerCollision(enemyLayer, boundaryLayer, true);
    }

    public override void Update()
    {
        base.Update();


        if (enemy.IsPlayerDetected())
        {

            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {

            if (stateTimer < 0 || Vector2.Distance(enemy.transform.position, player.position) > 10)
                stateMachine.ChangeState(enemy.idleState);
        }

        if(enemy.IsPlayerDetected().distance > enemy.attackDistance || !enemy.IsPlayerDetected())

            MoveTowardsPlayer();

    }

    private void MoveTowardsPlayer()
    {
        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }


    private bool PlayerHidden()
    {
        return enemy.IsPlayerDetected().distance > enemy.WallDetectedDistance();
    }

    public override void Exit()
    {
        base.Exit();
        Physics2D.IgnoreLayerCollision(enemyLayer, boundaryLayer, false);
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }
}
