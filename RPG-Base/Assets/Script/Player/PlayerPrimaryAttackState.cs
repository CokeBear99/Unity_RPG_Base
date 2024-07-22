using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCounter;
    private float lastTimeAttacked;
    private float comboWindow = 2;


    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        xInput = 0; // fix attackDir bug

        if(comboCounter >2 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }
    
        player.anim.SetInteger("ComboCounter",comboCounter);
        player.anim.speed = 1.0f;

        float attackDir = player.facingDir;
        if(xInput != 0)
            attackDir = xInput;

        if (comboCounter < 2)
        {
            player.SetVelocity(player.attackMovement[comboCounter] * attackDir, rb.velocity.y);
        }
        else
        {
            player.SetVelocity(player.attackMovement[comboCounter] * attackDir, player.jumpForce * 0.1f);
        }


        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        //모션 개선 
        player.StartCoroutine("BusyFor", 0.2f);

        comboCounter++;
        lastTimeAttacked = Time.time;
        player.anim.speed = 1.0f;

    }

    public override void Update()
    {
        base.Update();

        if(stateTimer <0)
        {
            player.SetZeroVelocity();
        }

        if(triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }   
    }
}
