using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack info")]
    public float[] attackMovement;
    public float counterAttackDuration = 0.2f;

    public bool isBusy {  get; private set; }
    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;

    [Header("Collision info")]
    [SerializeField] protected Transform groundCheckBox;
    [SerializeField] protected Vector2 groundCheckBoxSize;
    [SerializeField] protected LayerMask whatIsStair;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    public float dashDir {  get; private set; } 


    public GameObject sword {  get; private set; }  


    #region State
    public PlayerStateMachine stateMachine {  get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState {  get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; } 
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }

    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerBlackholeState blackholeState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this,stateMachine,"Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        blackholeState = new PlayerBlackholeState(this, stateMachine, "Jump");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState); 
    }

    protected override void Update()
    {   
        base.Update();

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


        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            dashDir = Input.GetAxisRaw("Horizontal");
            
            if (dashDir == 0)
                dashDir = facingDir;
                
            stateMachine.ChangeState(dashState);
        }
    }


    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    public void ExitBlackholeState()
    {
        stateMachine.ChangeState(airState);
    }


    void DrawRotatedWireCube(Vector3 position, Vector3 size, float angle)
    {
        Vector3 halfSize = size / 2;

        // 각도에 따라 꼭지점 계산
        Vector3 topRight = position + Quaternion.Euler(0, 0, angle) * new Vector3(halfSize.x, halfSize.y, 0);
        Vector3 topLeft = position + Quaternion.Euler(0, 0, angle) * new Vector3(-halfSize.x, halfSize.y, 0);
        Vector3 bottomRight = position + Quaternion.Euler(0, 0, angle) * new Vector3(halfSize.x, -halfSize.y, 0);
        Vector3 bottomLeft = position + Quaternion.Euler(0, 0, angle) * new Vector3(-halfSize.x, -halfSize.y, 0);

        // 사각형의 선 그리기
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    // OverlapBoxAll의 결과가 하나 이상 존재하는지 확인
    public bool IsStairDetected()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheckBox.position, groundCheckBoxSize, 30f * facingDir, whatIsStair);

        if (colliders.Length > 0)
            return true;
        else return false;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (rb == null) 
            return;
        else
        {
            DrawRotatedWireCube(groundCheckBox.position, groundCheckBoxSize, 30f * facingDir);
        }


    }



}
