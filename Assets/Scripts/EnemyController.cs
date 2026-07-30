using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //public float moveSpeed;
    //public Rigidbody rb;
    private bool chasing;
    public float distanceToChase = 10f, distanceToLose = 15f, distanceToStop = 2f, distanceToDestroy = 10f;
    private Vector3 targetPoint, originalPoint; 

    public NavMeshAgent agent;

    public float keepChasingTime = 5f;
    private float chaseCounter;

    [Header("Enemy Bullet Pool")]
    public BulletController bulletPrefab;
    public int bulletPoolSize = 20;

    private Queue<BulletController> bulletPool = new Queue<BulletController>();
    private Transform bulletPoolParent;
    private bool bulletPoolReady;

    public Transform firePoint;

    public float fireRate, waitBetweenShots = 2f, timeToShoot = 1f;
    private float fireCount, shotWaitCounter, ShootTimeCounter;

    public Animator anim;

    public AudioClip ShootSfx;

    //knockback variables
    private float knockbackTimer;
    private Vector3 knockbackVel;
    public float KnockbackForce = 8f;

    //decide the enemy is melee or not
    public bool isMelee = false;

    private bool isDead = false;

    public GameObject HealthBarGO;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPoint = transform.position;

        ShootTimeCounter = timeToShoot;
        shotWaitCounter = waitBetweenShots;

        PrepareBulletPool();
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;//replacing his y target to be his y axis itself

        if (isDead)
        {
            if (Vector3.Distance(transform.position, targetPoint) >= distanceToDestroy)
            {
                Destroy(gameObject);
            }
            return;
        }

        //knockback
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            agent.Move(knockbackVel * Time.deltaTime);
            return;//skip follow player
        }

        if (!chasing)//chasing is false
        {
            if (chaseCounter > 0)
            {
                agent.destination = transform.position;//stop at his current position
                chaseCounter -= Time.deltaTime;//counting down
            }
            else//when chase counter equals to 0
            {
                agent.destination = originalPoint;//go back to starting position
            }

            if (Vector3.Distance(transform.position, targetPoint) <= distanceToChase)//within chasing distance
            {
                chasing = true;

                ShootTimeCounter = timeToShoot;
                shotWaitCounter = waitBetweenShots;
            }

            if (agent.remainingDistance < 0.25f)
            {
                anim.SetBool("isMoving", false);
            }
            else
            {
                anim.SetBool("isMoving", true);
            }
        }
        else//chasing is true, he is chasing us here
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPoint - transform.position), Time.deltaTime * 10f);

            if (Vector3.Distance(transform.position, targetPoint) <= distanceToStop)//distance within 2m
            {
                agent.destination = transform.position;//stop at his current position
            }
            else//more than 2m
            {
                agent.destination = targetPoint;//chase the player
            }

            if (Vector3.Distance(transform.position, targetPoint) > distanceToLose)//out of chasing distance
            {
                chasing = false;

                chaseCounter = keepChasingTime;
            }


            if (shotWaitCounter > 0)
            {
                shotWaitCounter -= Time.deltaTime;

                if (shotWaitCounter <= 0)
                {
                    ShootTimeCounter = timeToShoot;
                }

                anim.SetBool("isMoving", true);
            }
            else if (PlayerController.instance.gameObject.activeInHierarchy)//just proceed the shooting when the player is active only
            {
                ShootTimeCounter -= Time.deltaTime;

                if (ShootTimeCounter > 0)//shoot within shootTimeCounter period
                {
                    fireCount -= Time.deltaTime;

                    if (fireCount <= 0)
                    {
                        fireCount = fireRate;
                        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

                        firePoint.LookAt(PlayerController.instance.transform.position + new Vector3(0f, 0.4f, 0f));
                        Vector3 targetDir = PlayerController.instance.transform.position - transform.position;//get direction
                        float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);//measuring the angle towards player

                        //only shoot when angle is less than 30
                        //for melee enemy, it will only attack when it reach the distance to stop
                        if (Math.Abs(angle) <= 30 && (!isMelee || Vector3.Distance(transform.position, targetPoint) <= distanceToStop))
                        {
                            // Instantiate(bullet, firePoint.position, firePoint.rotation);
                            GetBullet(firePoint.position, firePoint.rotation);

                            anim.SetTrigger("fireShot");
                        }
                        else
                        {
                            shotWaitCounter = waitBetweenShots;
                        }
                    }

                    agent.destination = transform.position;//stop while shooting
                }
                else
                {
                    shotWaitCounter = waitBetweenShots;
                }

                anim.SetBool("isMoving", false);
            }
        }
    }

    private void PrepareBulletPool()
    {
        if (bulletPoolReady) return;

        GameObject parentObj = new GameObject(gameObject.name + "_EnemyBulletPool");
        bulletPoolParent = parentObj.transform;

        for (int i = 0; i < bulletPoolSize; i++)
        {
            BulletController bullet = Instantiate(bulletPrefab, bulletPoolParent);
            bullet.gameObject.SetActive(false);
            bullet.SetReturnAction(ReturnBullet);
            bulletPool.Enqueue(bullet);
        }

        bulletPoolReady = true;

        Debug.Log(gameObject.name + " enemy bullet pool created.");
    }

    private BulletController GetBullet(Vector3 position, Quaternion rotation)
    {
        PrepareBulletPool();

        BulletController bullet;

        if (bulletPool.Count > 0)
        {
            bullet = bulletPool.Dequeue();
        }
        else
        {
            bullet = Instantiate(bulletPrefab, bulletPoolParent);
            PlayerController.instance.audiosource.PlayOneShot(ShootSfx, 0.6f);
            bullet.SetReturnAction(ReturnBullet);
        }

        bullet.shooter = transform;

        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.gameObject.SetActive(true);
        bullet.Fire();

        return bullet;
    }

    private void ReturnBullet(BulletController bullet)
    {
        bullet.gameObject.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    public void ApplyKnockback(Vector3 hitPosition, float force)
    {
        // calculate knockback direction
        Vector3 dir = transform.position - hitPosition;
        dir.y = 0; //lock y axis
        dir.Normalize();

        //calculate knockback power
        knockbackVel = dir * force;
        knockbackTimer = force * 0.02f;
    }

    public void Dead()
    {
        agent.enabled = false;
        anim.SetTrigger("Dead");
        isDead = true;
        Collider hitbox = GetComponentInChildren<Collider>();
        hitbox.enabled = false;
        HealthBarGO.SetActive(false);

    }

    public void GetHitAnim()
    {
        if(!isDead)
        {
            agent.isStopped = true;
            anim.SetTrigger("GetHit");
        }  
    }
}