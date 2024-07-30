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

        // ������
        if (Input.GetKey(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        // ���� ������ idle�� ��ȯ
        if(player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }

        // ���Ʒ� �̵� �Է��� ������� �ڵ����� ������
        if(yInput < 0 )
        {
            rb.velocity = new Vector2(0, yInput * player.moveSpeed * 0.5f);
        }
        else // �Է� ���ų� ���� ����Ű �Է½�
        {
            rb.velocity = new Vector2(0, rb.velocity.y * 0.7f);
        }

        // ������ ����Ű �Է����� �������� 
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
