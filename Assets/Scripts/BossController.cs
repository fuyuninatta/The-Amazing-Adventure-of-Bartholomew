using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour, IDamagable
{
    public int currentHealth, MaxHealth = 10;
    private bool phase2 = false, died=false;

    public float meleeDistance = 5f;
    public GameObject hitbox;
    public GameObject FinalGate;

    public float fireDistance = 12f;
    public Transform firePoint;
    private float fireCounter;
    public float fireRate = 1.5f;
    private bool isAttacking = false;

    public AudioClip ScreamSfx, BiteSfx, ShootFireSfx, GetHitSfx, DeathSfx;

    public NavMeshAgent agent;
    public Animator animator;

    [Header("Boss Bullet Pool")]
    public BulletController firePrefab;
    public int firePoolSize = 20;
    private Queue<BulletController> firePool = new Queue<BulletController>();
    private Transform firePoolParent;
    private bool firePoolReady;

    //original position and rotation
    private Vector3 originalPos;
    private Quaternion originalRot;

    //phase 2 variables
    private int phase2action = 0; 
    private float phase2Timer = 0f, actionDuration = 5f, freeroamRange = 10f;
    private bool isReturning = false;

    //UI
    private HealthBar healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = MaxHealth;
        PrepareFirePool();
        originalPos = transform.position;
        originalRot = transform.rotation;

        //get UI
        healthBar = gameObject.GetComponent<HealthBar>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = PlayerController.instance.transform.position;
        if (playerPos == null) return;//prevent error

        if (fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;
        }

        if(died) return;

        if (!phase2)
        {
            //phase 1 (melee attack or shoot fire or chase)
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

            resetanimTrigger();//reset all animation Trigger to prevent error

            if (Vector3.Distance(transform.position, playerPos) <= meleeDistance)//if player r too near to boss, melee attack
            {
                isAttacking = true;
                agent.velocity = Vector3.zero;
                agent.destination = transform.position;
                //either bite attack
                animator.SetTrigger("Bite");

            }
            else if (Vector3.Distance(transform.position, playerPos) <= fireDistance)//shoot fire
            {
                isAttacking = true;
                agent.velocity = Vector3.zero;
                Vector3 lookDir = new Vector3(playerPos.x, transform.position.y, playerPos.z);
                Quaternion targetRotation = Quaternion.LookRotation(lookDir - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

                if (fireCounter <= 0)
                {
                    animator.SetTrigger("GroundFire");
                    Invoke("Fire", 0.4f);
                    agent.destination = transform.position;
                    fireCounter = fireRate;
                }
            }
            else//chase player
            {
                agent.destination = playerPos;
            }

            animator.SetFloat("Speed", agent.velocity.magnitude);

            //change to phase two if current health is half
            if (currentHealth <= (MaxHealth / 2))
            {
                EnterPhase2();
                agent.speed *= 5;//increase speed for flying
            }
        }
        else//phase 2
        {
            if (isReturning)//returning to origin point to summon minions
            {
                agent.isStopped = false;

                if (!agent.hasPath || agent.remainingDistance < 0.5f)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                    agent.velocity = Vector3.zero;

                   if (!IsInvoking("ResetReturn"))
                    {
                        Debug.Log("summon");
                        animator.SetTrigger("Land");
                        Invoke("SummonMinions", 0.9f);
                        Invoke("ResetReturn", 0.9f);
                        phase2Timer = actionDuration;//reset action timer
                    }
                }
                else
                {
                    agent.destination = originalPos;
                    return;
                }
                return;
            }

            if (isAttacking)
            {
                transform.LookAt(playerPos);
                agent.isStopped = true;
                if (fireCounter <= 0)
                {
                    animator.SetTrigger("AirFire");
                    Invoke("Fire", 0.4f);
                    Invoke("Fire", 0.6f);
                    Invoke("Fire", 0.8f);
                    fireCounter = fireRate;
                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fly Float"))//if current animation is idle set isattacking to false
                {
                    isAttacking = false;
                    agent.isStopped = false;
                    phase2Timer = actionDuration;
                }
                else
                {
                    return;
                }
            }

            if (phase2Timer > 0)
            {
                phase2Timer -= Time.deltaTime;
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fly Float"))
                {
                    agent.isStopped = false;
                    if (!agent.hasPath || agent.remainingDistance < 0.5f)
                    {
                        Vector2 randPos = Random.insideUnitCircle * freeroamRange;//random position for freeroam
                        agent.destination = playerPos + new Vector3(randPos.x, 0f, randPos.y);
                    }
                }
            }
            else
            {
                agent.ResetPath();

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fly Float"))
                {
                    resetanimTrigger();//reset all animation Trigger to prevent error

                    Phase2Action();
                    if (phase2action == 1)//shoot 3 fire balls
                    {
                        Debug.Log("shootfire");
                        isAttacking = true; 
                    }
                    else if (phase2action == 2)//summon enemies(Land>Scream>Fly again)
                    {
                        Debug.Log("Returning");
                        isReturning = true;
                    }
                }
            }
        }
    }

    public void TakeDamage(int damage, bool attackPlayer)
    {
        if (!attackPlayer)
        {
            currentHealth -= damage;
            PlayerController.instance.audiosource.PlayOneShot(GetHitSfx, 0.2f);

            float hitChance = Random.value; //0.0 - 1.0
            if (hitChance < 0.3f)
            {
                if(phase2)
                {
                    animator.SetTrigger("GetHit2");
                }
                else
                {
                    animator.SetTrigger("GetHit1");
                }
                
            }

            if (currentHealth <= 0)
            {
                PlayerController.instance.audiosource.PlayOneShot(DeathSfx, 0.2f);
                Died();
            }
        }
    }

    public void EnterPhase2()//scream and fly
    {
        if (!phase2)//prevent repeat set phase2 as true
        {
            phase2 = true;
        }
        isAttacking = false;
        SummonMinions();
        agent.destination = transform.position;//stop
    }


    //ranndom pick a number to decide what boss will do
    //phase2action = (1:shoot fire, 2:summon minions)
    public void Phase2Action()
    {
        float randomVal = Random.value;//0.0 - 1.0

        if (randomVal < 0.7f)//70% chance to shoot
        {
            phase2action = 1;
        }
        else//30% chance to summon minions
        {
            phase2action = 2;
        }
    }

    public void SummonMinions()
    {
        animator.SetTrigger("Scream");
        PlayerController.instance.audiosource.PlayOneShot(ScreamSfx,0.6f);
        SummonEnemy.instance.spawnEnemy();

    }
    public void Died()
    {
        agent.enabled = false;
        animator.SetTrigger("Land");
        animator.SetBool("Dead",true);
        died = true;
        hitbox.SetActive(false);
        FinalGate.SetActive(true);        
    }

    public void resetanimTrigger()
    {
        foreach (var param in animator.parameters)
        {
            //if param.type is trigger
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }

    public void ResetReturn()
    {
        isReturning = false;
        isAttacking = false;    
    }

    private void Fire()
    {
        Vector3 targetPos = PlayerController.instance.transform.position + new Vector3(0f, 0.4f, 0f);
        firePoint.LookAt(targetPos);

        BulletController fire = GetFireFromPool(firePoint.position, firePoint.rotation);
        fire.shooter = transform;
        PlayerController.instance.audiosource.PlayOneShot(ShootFireSfx, 0.2f);
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
        fire.Fire();

        return fire;
    }

    private void ReturnFireToPool(BulletController fire)
    {
        fire.gameObject.SetActive(false);
        firePool.Enqueue(fire);
    }
}
