using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
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

    private float targetAlpha = 1f; // 목표 알파값

    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fX { get; private set; }
    #endregion
    public SpriteRenderer sr { get; private set; }


    public int facingDir { get; private set; } = 1; // 1 => 오른쪽 | -1 => 왼쪽
    protected bool facingRight = true;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        fX = GetComponent<EntityFX>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        #region ColorAlphaSet
        // 현재 색상 가져오기
        Color currentColor = sr.color;
        // 알파값을 목표 알파값으로 서서히 변경
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, 25f * Time.deltaTime);
        // 색상 업데이트
        sr.color = currentColor;
        #endregion
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

    // 수정필요
    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    public bool IsWallDetected() => Physics2D.Raycast(wallcheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsWall);

    public bool IsWallEndDetected() => Physics2D.Raycast(new Vector2(wallcheck.position.x, wallcheck.position.y + 0.9f), Vector2.right * facingDir, wallCheckDistance, whatIsWall);

    


    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(wallcheck.position, new Vector3(wallcheck.position.x + wallCheckDistance * facingDir, wallcheck.position.y));
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

    public void MakeTransparent(bool _transparent)
    {
        if (_transparent)
        {
            targetAlpha = 0f; // 투명하게 만들기
        }
        else
        {
            targetAlpha = 1f; // 불투명하게 만들기
        }
    }

}   
