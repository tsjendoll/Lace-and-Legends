using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Helpers.GetFilteredColliders(player.attackCheck.position, player.attackCheckRadius, player.facingDir);

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().Damage(player.facingDir);
        }
    
    }

    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}
