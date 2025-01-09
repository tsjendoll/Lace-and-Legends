using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private float returnSpeed;
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

    /// <summary>
    /// Initializes the components.
    /// </summary>
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    /// <summary>
    /// Destroys the sword.
    /// </summary>
    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets up the sword with initial parameters.
    /// </summary>
    /// <param name="_dir">Direction of the sword.</param>
    /// <param name="_gravityScale">Gravity scale of the sword.</param>
    /// <param name="_player">Player object.</param>
    /// <param name="_freezeDuration">Duration to freeze the enemy.</param>
    /// <param name="_returnSpeed">Speed to return the sword.</param>
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

        Invoke("DestroyMe", 7);
    }

    /// <summary>
    /// Sets up the bounce parameters.
    /// </summary>
    /// <param name="_isbouncing">Is the sword bouncing.</param>
    /// <param name="_amountOfBounce">Amount of bounces.</param>
    /// <param name="_bounceSpeed">Speed of the bounce.</param>
    public void SetupBounce(bool _isbouncing, int _amountOfBounce, float _bounceSpeed)
    {
        isBouncing = _isbouncing;
        bounceAmount = _amountOfBounce;
        bounceSpeed = _bounceSpeed;
    }

    /// <summary>
    /// Sets up the pierce parameters.
    /// </summary>
    /// <param name="_amountOfPierce">Amount of pierces.</param>
    public void SetupPierce(int _amountOfPierce)
    {
        pierceAmount = _amountOfPierce;
    }

    /// <summary>
    /// Sets up the spin parameters.
    /// </summary>
    /// <param name="isSpinning">Is the sword spinning.</param>
    /// <param name="maxTravelDistance">Maximum travel distance.</param>
    /// <param name="spinDuration">Duration of the spin.</param>
    /// <param name="hitCooldown">Cooldown between hits.</param>
    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }
    
    /// <summary>
    /// Returns the sword to the player.
    /// </summary>
    public void ReturnSword()
    {
        if (!canReturn)
            return;

        rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }

    /// <summary>
    /// Updates the sword's state every frame.
    /// </summary>
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

    /// <summary>
    /// Handles the logic for spinning the sword.
    /// </summary>
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
                            Enemy enemy = hit.GetComponent<Enemy>();
                            HitEnemy(enemy, 0);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Stops the sword when it is spinning.
    /// </summary>
    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }
    
    /// <summary>
    /// Handles the logic for bouncing the sword.
    /// </summary>
    private void BounceLogic()
    {
        if (isBouncing && enemyTargets.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTargets[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTargets[targetIndex].position) < .1f)
            {
                Enemy enemy = enemyTargets[targetIndex].GetComponent<Enemy>();
                int attackDirection = enemyTargets[targetIndex].position.x > transform.position.x ? 1 : -1;
                
                HitEnemy(enemy, attackDirection);

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

    /// <summary>
    /// Handles the logic when the sword collides with another object.
    /// </summary>
    /// <param name="collision">The collider of the object the sword collided with.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;
        
        if (collision.GetComponent<Enemy>() != null && !isBouncing && !isSpinning)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            int attackDirection = collision.transform.position.x > collision.bounds.center.x ? 1 : -1;

            HitEnemy(enemy, attackDirection);
        }

        SetupTargetsForBounce(collision);

        StuckInto(collision);
    }

    /// <summary>
    /// Hits the enemy and applies damage and freeze effect.
    /// </summary>
    /// <param name="enemy">The enemy to hit.</param>
    /// <param name="attackDirection">The direction of the attack.</param>
    private void HitEnemy(Enemy enemy, int attackDirection)
    {
        if (attackDirection == 0)
            attackDirection = UnityEngine.Random.value > 0.5f ? 1 : -1;
        enemy.Damage(attackDirection);
        enemy.FreezeTimeFor(freezeDuration);
    }

    /// <summary>
    /// Sets up the targets for bouncing.
    /// </summary>
    /// <param name="collision">The collider of the object the sword collided with.</param>
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

    /// <summary>
    /// Handles the logic when the sword gets stuck into an object.
    /// </summary>
    /// <param name="collision">The collider of the object the sword collided with.</param>
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
