using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneAnimationTriggers : MonoBehaviour
{
    private Clone_Skill_Controller cloneSkillController => GetComponentInParent<Clone_Skill_Controller>();

    private void AttackTrigger() => cloneSkillController.AttackTrigger();
}
