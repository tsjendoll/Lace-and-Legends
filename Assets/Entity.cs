using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Collision Info")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsSolid;
    [SerializeField] protected LayerMask whatIsWall;

    #region Components
    public List<Animator> anims { get; private set; }
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public int facingDir {get; private set; } = -1;
    protected bool facingRight = false;
    #endregion

    public string lastAnimBoolName { get; private set; }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        anims = new List<Animator>(GetComponentsInChildren<Animator>());
        rb = GetComponent<Rigidbody2D>();

        anim = anims.Count > 0 ? anims[0] : null; // Set anim to the first Animator, or null if none exist
    }

    protected virtual void Update()
    {

    }

    #region Velocity
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }

    public void setZeroVelocity()
    {
        rb.velocity = new Vector2(0, 0);
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsSolidDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsSolid);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsWall);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));

    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }
    #endregion

    public virtual void AssignLastAnimName(string _animBoolName) => lastAnimBoolName = _animBoolName;
}
