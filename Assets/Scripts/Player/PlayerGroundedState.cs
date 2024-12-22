using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    private RuntimeAnimatorController femaleCorsetAC;
    private RuntimeAnimatorController femalePantiesAndBraAC;
    private RuntimeAnimatorController femaleSkirtAC;


    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // Load the animation controllers
        femaleCorsetAC = Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clothing/Female/FemaleCorset_AC");
        femalePantiesAndBraAC = Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clothing/Female/FemalePantiesAndBra_AC");
        femaleSkirtAC = Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clothing/Female/FemaleSkirt_AC");
    }

    public override void Enter()
    {
        base.Enter();
        
        if (player.IsGroundDetected())
        {
            player.airDashCount = 0;
            player.jumpCount = 0;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Q))
            stateMachine.ChangeState(player.counterAttackState);

        if(Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.attackState);

        if (Input.GetKeyDown(KeyCode.Space) && (player.IsGroundDetected() || player.IsSolidDetected()))
            stateMachine.ChangeState(player.jumpState);

        // Switch animation controllers when a key is pressed (e.g., the "C" key)
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchClothingAnimation();
        }
    }

    private void SwitchClothingAnimation()
    {
        AnimatorStateInfo parentStateInfo = player.playerAnimator.GetCurrentAnimatorStateInfo(0);
        

        if (player.IsCorsetActive)
        {
            player.topAnimator.runtimeAnimatorController = femalePantiesAndBraAC;
            player.bottomAnimator.runtimeAnimatorController = femalePantiesAndBraAC;
            
        }
        else
        {
            player.topAnimator.runtimeAnimatorController = femaleCorsetAC;
            player.bottomAnimator.runtimeAnimatorController = femaleSkirtAC;
        }

        player.IsCorsetActive = !player.IsCorsetActive;
        
        Helpers.SyncAnimators(player.topAnimator, parentStateInfo);
        Helpers.SyncAnimators(player.bottomAnimator, parentStateInfo);
    }
}
