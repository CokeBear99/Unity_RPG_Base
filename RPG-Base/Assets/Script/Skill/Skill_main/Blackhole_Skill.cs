using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill : Skill
{
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [Space]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int amountAttacks;
    [SerializeField] private float blackholeDuration;

    Blackhole_Skill_Controller newBlackholeController;

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackhole = Instantiate(blackholePrefab,player.transform.position,Quaternion.identity );
        newBlackholeController = newBlackhole.GetComponent<Blackhole_Skill_Controller>();
        
        newBlackholeController.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountAttacks, attackCooldown,blackholeDuration);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public float GetCooldownTimer()
    {
        return cooldownTimer;
    }


    public bool BlackholeExit()
    {
        if (!newBlackholeController)
            return false;


        if(newBlackholeController.ExitState == true)
        {
            newBlackholeController = null;
            return true;
        }


        return false;
    }

}
