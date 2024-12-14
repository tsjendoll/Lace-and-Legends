using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    private RuntimeAnimatorController femaleCorsetAC;
    private RuntimeAnimatorController femalePantiesAndBraAC;
    private RuntimeAnimatorController femaleSkirtAC;
    private bool isCorsetActive;

    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // Load the animation controllers
        femaleCorsetAC = Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clothing/Female/FemaleCorset_AC");
        femalePantiesAndBraAC = Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clothing/Female/FemalePantiesAndBra_AC");
        femaleSkirtAC = Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clothing/Female/FemaleSkirt_AC");


        // Initialize the current active controller
        isCorsetActive = true;
    }

    public override void Enter()
    {
        base.Enter();
        player.doubleJump = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

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
        

        if (isCorsetActive)
        {
            player.topAnimator.runtimeAnimatorController = femalePantiesAndBraAC;
            player.bottomAnimator.runtimeAnimatorController = femalePantiesAndBraAC;
            
        }
        else
        {
            player.topAnimator.runtimeAnimatorController = femaleCorsetAC;
            player.bottomAnimator.runtimeAnimatorController = femaleSkirtAC;
        }

        isCorsetActive = !isCorsetActive;
        
        // player.topAnimator.SetBool("Idle", true);
        // player.topAnimator.Play(parentStateInfo.fullPathHash, -1, parentAnimatorTime);
        // player.topAnimator.Update(0); // Ensure the animator updates to the correct state immediately

        // player.bottomAnimator.SetBool("Idle", true);
        // player.bottomAnimator.Play(parentStateInfo.fullPathHash, -1, parentAnimatorTime);
        // player.bottomAnimator.Update(0);

        SyncAnimators(player.topAnimator, parentStateInfo);
        SyncAnimators(player.bottomAnimator, parentStateInfo);
        
    }

    private void SyncAnimators(Animator _animator, AnimatorStateInfo _parentStateInfo) {
        float parentAnimatorTime = _parentStateInfo.normalizedTime % 1;

        _animator.SetBool("Idle", true);
        _animator.Play(_parentStateInfo.fullPathHash, -1, parentAnimatorTime);
        _animator.Update(0);

    }
}
