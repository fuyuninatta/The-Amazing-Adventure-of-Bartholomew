using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour, IDamagable
{
    public int currentHealth, MaxHealth = 10;
    private bool phase2 = false;

    public float meleeDistance = 6f;
    public float fireDistance = 12f;
    public Transform firePoint;
    private float fireCounter;
    public float fireRate = 1.5f;
    private bool isAttacking = false;

    public AudioClip ScreamSfx, ShootFireSfx,GetHitSfx, DeathSfx;

    public NavMeshAgent agent;
    public Animator animator;

    [Header("Boss Bullet Pool")]
    public BulletController firePrefab;
    public int firePoolSize = 20;
    private Queue<BulletController> firePool = new Queue<BulletController>();
    private Transform firePoolParent;
    private bool firePoolReady;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = MaxHealth;
        PrepareFirePool();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 player = PlayerController.instance.transform.position;
        if (player == null) return;//prevent error

        if (fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;
        }

        if (!phase2)
        {
            if(isAttacking)//if attacking stop repeating the attack animation
            {
                if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))//if current animation is idle set isattacking to false
                {
                    isAttacking = false;
                }
                else
                {
                    return;
                }
            }

            if (Vector3.Distance(transform.position, player) <= meleeDistance)//if player r too near to boss, melee attack
            {
                isAttacking = true;
                agent.destination = transform.position;
                //either bite attack
                animator.SetTrigger("Bite");
            }
            else if (Vector3.Distance(transform.position, player) <= fireDistance)//shoot fire
            {
                isAttacking = true;
                agent.destination = transform.position;

                transform.LookAt(new Vector3(player.x, transform.position.y, player.y));
                firePoint.LookAt(player + new Vector3(0f, 0.4f, 0f));

                if (fireCounter <= 0)
                {
                    animator.SetTrigger("GroundFire");
                    Invoke("Fire", 0.4f);

                    fireCounter = fireRate;
                }
            }
            else//chase player
            {
                agent.SetDestination(player);
            }

            animator.SetFloat("Speed", agent.velocity.magnitude);

            //change to phase two if current health is half
            if (currentHealth <= (MaxHealth / 2))
            {
                EnterPhase2();
            }
        }
        else
        {
            //face to player on air
            Vector3 targetPos = new Vector3(player.x, player.y + 4f, player.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 4f);

            Vector3 dir = player - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 3f);

            return;
        }
    }

    public void TakeDamage(int damage, bool attackPlayer)
    {
        if (!attackPlayer)
        {
            currentHealth -= damage;
            PlayerController.instance.audiosource.PlayOneShot(GetHitSfx, 0.2f);
            if (currentHealth <= 0)
            {
                PlayerController.instance.audiosource.PlayOneShot(DeathSfx, 0.2f);
                Died();
            }
        }
    }

    public void EnterPhase2()
    {
        phase2 = true;
        animator.SetBool("Phase2", true);
        //stop agent
        agent.isStopped = true;
        agent.enabled = false;
        animator.SetTrigger("Fly");
    }

    public void Died()
    {
        agent.enabled = false;
        animator.SetTrigger("Dead");
    }

    private void PrepareFirePool()
    {
        if (firePoolReady) return;

        GameObject parentObj = new GameObject(gameObject.name + "_BossFirePool");
        firePoolParent = parentObj.transform;

        for (int i = 0; i < firePoolSize; i++)
        {
            BulletController fire = Instantiate(firePrefab, firePoolParent);
            fire.gameObject.SetActive(false);
            fire.SetReturnAction(ReturnFireToPool);
            firePool.Enqueue(fire);
        }

        firePoolReady = true;
    }

    private BulletController GetFireFromPool(Vector3 position, Quaternion rotation)
    {
        PrepareFirePool();

        BulletController fire;

        if (firePool.Count > 0)
        {
            fire = firePool.Dequeue();
        }
        else
        {
            fire = Instantiate(firePrefab, firePoolParent);
            fire.SetReturnAction(ReturnFireToPool);
        }

        fire.transform.SetPositionAndRotation(position, rotation);
        fire.gameObject.SetActive(true);
        fire.Fire(); // 触发 BulletController 内部的移动/飞行逻辑

        return fire;
    }

    private void ReturnFireToPool(BulletController fire)
    {
        fire.gameObject.SetActive(false);
        firePool.Enqueue(fire);
    }

    private void Fire()
    {
        BulletController fire = GetFireFromPool(firePoint.position, firePoint.rotation);
        fire.shooter = transform;
        PlayerController.instance.audiosource.PlayOneShot(ShootFireSfx, 0.2f);
    }
}
