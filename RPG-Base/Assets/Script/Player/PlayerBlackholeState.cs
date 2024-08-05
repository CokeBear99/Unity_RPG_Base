using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = 0.4f;
    private bool skillUsed;
    private float defaultGravity;

    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = rb.gravityScale;
        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;

    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = defaultGravity;
        PlayerManager.instance.player.MakeTransparent(false);
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 10);
        }

        if(stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -0.1f);

            if(!skillUsed)
            {
                if(SkillManager.instance.blackhole.CanUseSkill())
                    skillUsed = true;
            }   
        }


        if(SkillManager.instance.blackhole.BlackholeExit())
        {
            stateMachine.ChangeState(player.airState);
        }


    }
}
