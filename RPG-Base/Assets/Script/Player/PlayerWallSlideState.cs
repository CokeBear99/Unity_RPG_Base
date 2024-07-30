using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();


    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 벽점프
        if (Input.GetKey(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        // 땅에 닿으면 idle로 전환
        if(player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }

        // 위아래 이동 입력이 없을경우 자동으로 내려감
        if(yInput < 0 )
        {
            rb.velocity = new Vector2(0, yInput * player.moveSpeed * 0.5f);
        }
        else // 입력 없거나 위쪽 방향키 입력시
        {
            rb.velocity = new Vector2(0, rb.velocity.y * 0.7f);
        }

        // 벽에서 방향키 입력으로 떨어지기 
        if(xInput != 0 && player.facingDir != xInput)
        {
            stateMachine.ChangeState(player.idleState);
        }

        //
        if(player.IsWallEndDetected() == false)
        {
            stateMachine.ChangeState(player.airState);
        }


    }
}
