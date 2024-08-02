using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    private RuntimeAnimatorController femaleCorsetAC;
    private RuntimeAnimatorController femalePantiesAndBraAC;
    private bool isCorsetActive;

    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // Load the animation controllers
        femaleCorsetAC = Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clothing/Female/FemaleCorset_AC");
        femalePantiesAndBraAC = Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clothing/Female/FemalePantiesAndBra_AC");

        // Initialize the current active controller
        isCorsetActive = true;
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

        if (Input.GetKeyDown(KeyCode.Space))
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
        float parentAnimatorTime = parentStateInfo.normalizedTime % 1;

        if (isCorsetActive)
        {
            player.clothingAnimator.runtimeAnimatorController = femalePantiesAndBraAC;
            
        }
        else
        {
            player.clothingAnimator.runtimeAnimatorController = femaleCorsetAC;
        }

        isCorsetActive = !isCorsetActive;
        player.clothingAnimator.SetBool("Idle", true);

        player.clothingAnimator.Play(parentStateInfo.fullPathHash, -1, parentAnimatorTime);
        player.clothingAnimator.Update(0); // Ensure the animator updates to the correct state immediately
    
    }
}
