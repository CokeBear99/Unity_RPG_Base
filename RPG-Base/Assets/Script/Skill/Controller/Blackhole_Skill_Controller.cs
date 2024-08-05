using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;

    [SerializeField] private bool canGrow = true;
    [SerializeField] private bool canShrink;
    private bool canCreateHotKey = true;
    private bool canAttack;
    public bool ExitState {  get; private set; } 

    private int amountAttacks;
    private float cloneAttackCooldown;
    private float cloneAttackTimer;
    private float blackholeTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();


    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountAttacks, float _cloneAttackCooldown,float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountAttacks = _amountAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;

        blackholeTimer = _blackholeDuration;
    }


    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if(blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;

            if (targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackholeAbility();   
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            if(targets.Count <= 0)
            {
                FinishBlackholeAbility();
            }

            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        GrowAndShrink();

    }

    private void ReleaseCloneAttack()
    {
        DestroyHotkeys();
        canAttack = true;
        canCreateHotKey = false;

        PlayerManager.instance.player.MakeTransparent(true);
    }

    private void GrowAndShrink()
    {
        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);

            if (transform.localScale.x >= maxSize - 0.1f)
                canCreateHotKey = false;
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void CloneAttackLogic()
    {
        if (canAttack && cloneAttackTimer < 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;
            if (Random.Range(0, 100) > 50)
                xOffset = 1.5f;
            else
                xOffset = -1.5f;

            SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            amountAttacks--;

            if (amountAttacks <= 0)
            {
                Invoke("FinishBlackholeAbility", 0.5f);
            }
        }
    }

    private void FinishBlackholeAbility()
    {
        DestroyHotkeys();
        canShrink = true;
        canAttack = false;
        ExitState = true;
    }

    private void DestroyHotkeys()
    {
        if (createdHotKey.Count <= 0)
            return;

        for(int i =0; i < createdHotKey.Count;i++)
        {
            Destroy(createdHotKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);

        }



    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.Log("List count is 0");
            return;
        }

        if (!canCreateHotKey)
            return;

        GameObject newHotkey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotkey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        newHotkey.GetComponent<Blackhole_Hotkey_Controller>().SetupHotkey(choosenKey, collision.transform, this);
    }


    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);




}
