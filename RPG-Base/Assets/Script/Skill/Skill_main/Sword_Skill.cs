using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;


public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}



public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private bool isSpinning;
    [SerializeField] private float maxMoveDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;
    [SerializeField] private float hitCooldown = 0.2f;

    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;


    private Vector2 finalDir;

    [Header("Aim Dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBeetwenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        GenerateDots();

    }

    private void SetupGravity()
    {
        if(swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if(swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity; 
    }

    protected override void Update()
    {
        SetupGravity();

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);
            
            if(swordType == SwordType.Pierce)
                finalDir = new Vector2(AimDirection().normalized.x * launchForce.x * 1.7f, AimDirection().normalized.y * launchForce.y);
                
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for(int i=0; i<dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBeetwenDots);
            }

        }   
    }


    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab,player.transform.position,transform.rotation);

        if (swordType == SwordType.Bounce)
        {
            newSword.GetComponent<Sword_Skill_Controller>().SetupBounce(true, bounceAmount);
        }
        else if(swordType== SwordType.Pierce)
        {
            newSword.GetComponent<Sword_Skill_Controller>().SetupPierce(pierceAmount);
        }
        else if (swordType == SwordType.Spin)
        {
            isSpinning = true;
            newSword.GetComponent<Sword_Skill_Controller>().SetupSpin(isSpinning,maxMoveDistance,spinDuration,hitCooldown);
        }



        newSword.GetComponent<Sword_Skill_Controller>().SetupSword(finalDir, swordGravity,player);
        player.AssignNewSword(newSword);

        DotsActive(false);
    }


    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }


    #region DrawDots



    // Set Dots Active True
    public void DotsActive(bool _isActive)
    {
        for(int i = 0; i< dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }


    // Generate Dots -> Visible false
    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i< numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + 0.5f * (Physics2D.gravity * swordGravity) * (t * t);
    
        return position;
    }


    #endregion






}
