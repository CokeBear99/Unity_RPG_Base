using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if (player.IsStairDetected() == true)
        {
            rb.velocity = new Vector2(rb.velocity.x / 1.7f, rb.velocity.y);
        }

        /*
        if (player.IsStairDetected() == false)
        {
            player.SetVelocity(xInput * player.moveSpeed , rb.velocity.y);
        }
        else
        {
            player.SetVelocity(xInput * player.moveSpeed / 2f , rb.velocity.y );
        }
        */





        if (xInput == 0 || player.IsWallDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
