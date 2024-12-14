using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected LayerMask whatIsBoundary;

 
    [Header("Stunned Info")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;


    [Header("Move Info")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;

    [Header("Attack Info")]
    public float attackDistance;
    public float attackCooldown;
    public float minAttackCooldown;
    public float maxAttackCooldown;
    [HideInInspector] public float lastTimeAttacked;

    public EnemyStateMachine stateMachine { get; private set; }
    // public EntityFX fx { get; private set; }
    

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        defaultMoveSpeed = moveSpeed;
        
    }

    protected override void Start()
    {
        base.Start();
        // facingRight = true;
        Flip();
        // fx = GetComponent<EntityFX>();
    }

    public override bool IsWallDetected()
    {
        return base.IsWallDetected() || Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsBoundary);
    }
    
    protected override void Update()
    {
        // if (ui.IsMenuOpen())
        //     return;
            
        base.Update();
        stateMachine.currentState.Update();
        if (transform.position.y < -20)
            Destroy(this.gameObject);
    }

    // public override void DamageEffect(bool _knockback)
    // {
    //     fx.StartCoroutine("FlashFX");
        
    //     base.DamageEffect(_knockback);   
    // }

    

    // public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    // {
    //     moveSpeed = moveSpeed * (1 - _slowPercentage);
    //     anim.speed = anim.speed * (1 - _slowPercentage);

    //     Invoke("ReturnDefaultSpeed", _slowDuration);
    // }

    // protected override void ReturnDefaultSpeed()
    // {
    //     base.ReturnDefaultSpeed();

    //     moveSpeed = defaultMoveSpeed;
    // }

    // public virtual void FreezeTime(bool _timeFrozen)
    // {
    //     if (_timeFrozen)
    //     {
    //         moveSpeed = 0;
    //         anim.speed = 0;
    //     }
    //     else if(!_timeFrozen)
    //     {
    //         moveSpeed = defaultMoveSpeed;
    //         anim.speed = 1;
    //     }
    // }

    // public virtual void FreezeTimeFor(float duration) => StartCoroutine(FreezeTimeCoroutine(duration));
    
    // protected virtual IEnumerator FreezeTimeCoroutine(float _seconds)
    // {
    //     FreezeTime(true);

    //     yield return new WaitForSeconds(_seconds);

    //     FreezeTime(false);
    // }
    
    #region Counter Attack Window
        
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion

    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }

        return false;
    }

    public void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir,  15, whatIsPlayer);

    public float WallDetectedDistance() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir,  50, whatIsWall).distance;
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }
}
