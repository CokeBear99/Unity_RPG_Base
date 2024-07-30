using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallcheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsWall;

    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float KnockbackDuration;
    protected bool isKnocked;

    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fX { get; private set; }
    #endregion



    public int facingDir { get; private set; } = 1; // 1 => ¿À¸¥ÂÊ | -1 => ¿ÞÂÊ
    protected bool facingRight = true;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fX = GetComponent<EntityFX>();
        
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        rb.velocity = new Vector2(knockbackDirection.x * -facingDir, knockbackDirection.y);

        yield return new WaitForSeconds(KnockbackDuration);
        isKnocked = false;
        yield return new WaitForSeconds(0.25f);
        SetZeroVelocity();
    }

    public virtual void Damage()
    {
        fX.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback");
        Debug.Log(gameObject.name + " was damaged!");
    }

    #region Collision
    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public bool IsWallDetected() => Physics2D.Raycast(wallcheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsWall);

    public bool IsWallEndDetected() => Physics2D.Raycast(new Vector2(wallcheck.position.x, wallcheck.position.y + 0.9f), Vector2.right * facingDir, wallCheckDistance, whatIsWall);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(wallcheck.position, new Vector3(wallcheck.position.x + wallCheckDistance, wallcheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region SettingVelocity
    public void SetZeroVelocity()
    {
        if (isKnocked) return;

        rb.velocity = new Vector2(0, 0);
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if(isKnocked) return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);

        FlipController(_xVelocity);
    }
    #endregion

    #region Flip
    public void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public void FlipController(float _x)
    {
        if (_x > 0 && facingRight == false)
        {
            Flip();
        }
        else if (_x < 0 && facingRight == true)
        {
            Flip();
        }
    }
    #endregion

}
