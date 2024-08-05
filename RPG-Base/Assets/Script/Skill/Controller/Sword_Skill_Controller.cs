using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    [SerializeField] private float returnSpeed;
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    [Header("Pierce info")]
    [SerializeField] private float pierceAmount;

    [Header("Bounce info")]
    [SerializeField]private float bounceSpeed;
    private bool isBouncing;
    [SerializeField]private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin info")]
    private float maxMoveDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }


    private void Update()
    {
        if (canRotate)
        {
            transform.right = rb.velocity;
        }

        SwordIsFallToPlayer();

        if (isReturning)
        {
            anim.SetBool("Rotation", true);
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 0.5)
            {
                anim.SetBool("Rotation", false);
                player.CatchTheSword();
            }
        }

        BounceLogic();

        SpinLogic();

    }

    private void SwordIsFallToPlayer()
    {
        if (player.sword != null && Vector2.Distance(player.transform.position, player.sword.transform.position) > 15)
        {
            Invoke("ReturnSword", 1.5f);
        }
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, this.transform.position) > maxMoveDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;
                if (hitTimer <= 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] Colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in Colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            hit.GetComponent<Enemy>().Damage();
                    }

                }


            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    public void SetupSpin(bool _isSpinning,float _maxMoveDistance,float _spinDuration,float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxMoveDistance = _maxMoveDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupBounce(bool _isBouncing, int _bounceAmount)
    {
        isBouncing = _isBouncing;
        bounceAmount = _bounceAmount;

        enemyTarget = new List<Transform>();
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
            {
                enemyTarget[targetIndex].GetComponent<Enemy>().Damage();

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    public void SetupSword(Vector2 _dir,float _gravityScale,Player _player)
    {
        player = _player;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        if(pierceAmount <= 0)
            anim.SetBool("Rotation",true);

    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        SetTargetsForBounce(collision);

        HitMonster(collision);

        StuckInto(collision);

    }

    private void SetTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] Colliders = Physics2D.OverlapCircleAll(transform.position, 5);

                foreach (var hit in Colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }

            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            if(pierceAmount != 1)
            {
                pierceAmount--;
                return;
            }
            else if( pierceAmount == 1)
            {
                pierceAmount--;
            }
        }

        if(isSpinning) { return; }


        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }












    private static void HitMonster(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            // Enemy 스크립트를 가져와서 Damage 메서드 호출
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Damage(); // Enemy의 Damage 메서드 실행
            }
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject); // 현재 객체 삭제
    }

}
