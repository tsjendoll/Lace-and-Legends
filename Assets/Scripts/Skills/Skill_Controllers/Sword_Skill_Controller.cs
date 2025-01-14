using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    [SerializeField] private float returnSpeed = 12;
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;
    private bool isReturning;
    private bool canReturn;

    private float freezeDuration;

    [Header("Bounce Info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;

    [Header("Pierce Info")]
    private int pierceAmount;

    [Header("Spin Info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;


    private List<Transform> enemyTargets = new List<Transform>();
    private int targetIndex;


    private bool canRotate = true;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeDuration, float _returnSpeed)
    {
        player = _player;
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        freezeDuration = _freezeDuration;
        returnSpeed = _returnSpeed;
        
        if (pierceAmount > 0)
            return;
        
        anim.SetBool("Rotate", true);
    }

    public void SetupBounce(bool _isbouncing, int _amountOfBounce, float _bounceSpeed)
    {
        isBouncing = _isbouncing;
        bounceAmount = _amountOfBounce;
        bounceSpeed = _bounceSpeed;
    }

    public void SetupPierce(int _amountOfPierce)
    {
        pierceAmount = _amountOfPierce;
    }

    public void SetupSpin(bool isSpinning, float maxTravelDistance, float spinDuration, float hitCooldown)
    {
        this.isSpinning = isSpinning;
        this.maxTravelDistance = maxTravelDistance;
        this.spinDuration = spinDuration;
        this.hitCooldown = hitCooldown;
    }
    
    public void ReturnSword()
    {
        if (!canReturn)
            return;

        rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) < .2)
            {
                player.CatchSword();
                isReturning = false;
                canReturn = false;
            }
        }

        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            hit.GetComponent<Enemy>().Damage(UnityEngine.Random.value > 0.5f ? 1 : -1);
                            Debug.Log("hit from Spin Logic");
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }
    
    private void BounceLogic()
    {
        if (isBouncing && enemyTargets.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTargets[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTargets[targetIndex].position) < .1f)
            {
                Enemy enemy = enemyTargets[targetIndex].GetComponent<Enemy>();
                int intAttackDirection = enemyTargets[targetIndex].position.x > transform.position.x ? 1 : -1;
                enemyTargets[targetIndex].GetComponent<Enemy>().Damage(intAttackDirection);
                Debug.Log("hit from Bounce Logic");

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTargets.Count)
                    targetIndex = 0;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;
        
        if (collision.GetComponent<Enemy>() != null && !isBouncing && !isSpinning)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            Vector2 attackDirection = (collision.transform.position - collision.bounds.center).normalized;
            int intAttackDirection = attackDirection.x > 0 ? 1 : -1;

            enemy.Damage(intAttackDirection);
            enemy.FreezeTimeFor(freezeDuration);
            Debug.Log("hit from OnTriggerEnter2D");
        }


        SetupTargetsForBounce(collision);

        StuckInto(collision);
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {

            if (isBouncing && enemyTargets.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTargets.Add(hit.transform);
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;

            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTargets.Count > 0)
            return;

        canReturn = true;

        anim.SetBool("Rotate", false);
        transform.parent = collision.transform;
    }

    
}
