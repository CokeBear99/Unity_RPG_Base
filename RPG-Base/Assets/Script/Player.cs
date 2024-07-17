using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Attack info")]
    public float[] attackMovement;

    public bool isBusy {  get; private set; }
    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    [SerializeField] private float dashCoolDown;
    private float dashUsageTimer;
    public float dashDir {  get; private set; } 

    [Header("Collision info")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform wallcheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;

    public int facingDir { get; private set; } = 1; // 1 => 오른쪽 | -1 => 왼쪽
    private bool facingRight = true;


    #region Components
    public Animator anim {  get; private set; }
    public Rigidbody2D rb { get; private set; }

    #endregion

    #region State
    public PlayerStateMachine stateMachine {  get; private set; } // 이 스크립트 내에서만 값 변경

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState {  get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; } 
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }

    #endregion

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this,stateMachine,"Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine.Initialize(idleState); 
    }

    private void Update()
    {   
        stateMachine.currentState.Update();

        CheckForDashInput();
        

    }

    public IEnumerable BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        if(IsWallDetected())
        {
            return; 
        }

        dashUsageTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer < 0)
        {
            dashUsageTimer = dashCoolDown;
            dashDir = Input.GetAxisRaw("Horizontal");
            
            if (dashDir == 0)
                dashDir = facingDir;
                
            stateMachine.ChangeState(dashState);
        }
    }

    #region SettingVelocity
    public void ZeroVelocity() => rb.velocity = new Vector2(0,0);

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);

        FlipController(_xVelocity);
    }
    #endregion

    #region Collision
    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public bool IsWallDetected() => Physics2D.Raycast(wallcheck.position,Vector2.right * facingDir , wallCheckDistance, whatIsWall);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(wallcheck.position, new Vector3(wallcheck.position.x + wallCheckDistance, wallcheck.position.y)); 
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
        if(_x > 0  && facingRight==false )
        {
            Flip();
        }
        else if( _x < 0  && facingRight==true )
        {
            Flip();
        }
    }
    #endregion

}
