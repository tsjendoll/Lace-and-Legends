using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private SpriteRenderer[] srs;
    private List<Animator> anims;
    [SerializeField] private float colorLossSpeed;
    
    private float cloneTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius;
    private int facingDir;

    private void Awake()
    {
        srs = GetComponentsInChildren<SpriteRenderer>();
        
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
        {
            // Calculate the new alpha value for all sprites
            if (srs.Length > 0)
            {
                float newAlpha = Mathf.Clamp(srs[0].color.a - (Time.deltaTime * colorLossSpeed), 0, 1);

                // Apply the new alpha to all sprites
                foreach (SpriteRenderer sr in srs)
                {
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha);
                }
            }
        }

        Destroy(gameObject, 3);
    }


    public void SetupClone(Transform _newTranform, float _cloneDuration, bool _canAttack)
    {
        anims = new List<Animator>(GetComponentsInChildren<Animator>());
        
        if (_canAttack)
        {
            foreach(var anim in anims)
            {
                anim.SetInteger("AttackNumber", 1);
            }

        }

        transform.position = _newTranform.position;
        FaceClosestTarget();
        cloneTimer = _cloneDuration;
    }

    private void AnimationTrigger()
    {
        cloneTimer = -1;
    }

    public void AttackTrigger()
    {
        Collider2D[] colliders = Helpers.GetFilteredColliders(attackCheck.position, attackCheckRadius, facingDir);

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().Damage(1);
        }
    
    }

    private void FaceClosestTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestTargetDistance = Mathf.Infinity;
        Enemy closestEnemy = null;

        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                Debug.Log("ENEMY detected");
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if (distanceToEnemy < closestTargetDistance)
                {
                    closestEnemy = hit.GetComponent<Enemy>();
                    closestTargetDistance = distanceToEnemy;
                }
            }
        }

        if (closestEnemy.transform.position.x < transform.position.x)
        {
            facingDir = -1;
        }
        else 
        {
            transform.Rotate(0, 180,0);
            facingDir = 1;
        }

    }
}
