using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //public float moveSpeed;
    //public Rigidbody RB;
    private bool chasing;
    public float distanceToChase = 10f, distanceToLose = 15f, distanceToStop = 2f;
    private Vector3 targetPoint,originalPosition;

    public NavMeshAgent agent;

    public float keepChasingTime = 5f;
    private float chaseCounter;

    public GameObject bullet;
    public Transform firePoint;

    public float fireRate, waitBetweenShots = 2f, timetoShoot = 1f;
    private float fireCount, shotWaitCounter, ShootTimeCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;

        ShootTimeCounter = timetoShoot;
        shotWaitCounter = waitBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;//replacing his y target to be his y axis itselfs

        if(!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) <= distanceToChase)
            {
                chasing = true;

                ShootTimeCounter = timetoShoot;
                shotWaitCounter = waitBetweenShots;
            }

            chaseCounter -= Time.deltaTime;
            if(chaseCounter<0)
            {
                agent.destination = originalPosition;
            }
        }
        else
        {
            //Method without NavMeshAgent
            //transform.LookAt(targetPoint);
            //RB.linearVelocity = transform.forward * moveSpeed;

            //chasing
            if(Vector3.Distance(transform.position, targetPoint) <= distanceToStop)
            {
                agent.destination = transform.position;
            }
            else
            {
                agent.destination = targetPoint;
            }

            //stop chasing   
            if (Vector3.Distance(transform.position, targetPoint) > distanceToLose)
            {
                chasing = false;

                chaseCounter = keepChasingTime;
            }


            //shot
            if(shotWaitCounter > 0)
            {
                shotWaitCounter -= Time.deltaTime;
                if(ShootTimeCounter<=0)
                {
                    ShootTimeCounter = timetoShoot;
                }
            }
            else
            {
                ShootTimeCounter -= Time.deltaTime;

                if(ShootTimeCounter>0)//shoot within shootTimerCounter
                {
                    fireCount -= Time.deltaTime;

                    if (fireCount <= 0)
                    {
                        fireCount = fireRate;

                        firePoint.LookAt(PlayerController.instance.transform.position + new Vector3(0f, 0.4f, 0f));

                        Vector3 targetDir = PlayerController.instance.transform.position - transform.position;//get Direction
                        float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

                        if(Math.Abs(angle) <= 30)//only shoot when angle is less than 30
                        {
                            Instantiate(bullet, firePoint.position, firePoint.rotation);
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
            }    
        }
    }
}
