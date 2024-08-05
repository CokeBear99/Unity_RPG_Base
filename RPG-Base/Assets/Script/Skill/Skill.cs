using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;

    protected Player player;


    protected virtual void Awake()
    {
        player = PlayerManager.instance.player;
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if( cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }
        else
        {
            Debug.Log(cooldownTimer + " seconds later can use Skill");
            return false;
        }
    }


    public virtual void UseSkill()
    {
        // do some skill things
    }


}
