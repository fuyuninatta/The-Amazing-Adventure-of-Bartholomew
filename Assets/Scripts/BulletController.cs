using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BulletController : MonoBehaviour
{
    public float moveSpeed, lifeTime;
    public Rigidbody rb;
    public GameObject impactEffect;
    // public bool damageEnemy, damagePlayer;
    // public int damageAmount;

    public bool attackPlayer;
    public int damage;

    private float lifeCounter;
    private Gun ownerGun;
    private bool hasHit;

    private Action<BulletController> returnToPool;

    //pierce bullet
    public int pierceCount = 0;//0 can not pierce, >0 can pierce
    private int remainingPierces;
    private List<Collider> hitList = new List<Collider>();//record how many enemy get hit

    public float KnockBackPower;
    
    public void SetReturnAction(Action<BulletController> returnAction)
    {
        returnToPool = returnAction;
    }

    public void Fire()
    {
        hasHit = false;
        lifeCounter = lifeTime;

        //reset pierce variables
        remainingPierces = pierceCount;
        hitList.Clear();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = transform.forward * moveSpeed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        lifeCounter -= Time.deltaTime;

        if (lifeCounter <= 0)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitList.Contains(other)) return;//prevent repeat hit
        

        IDamagable damageable = other.GetComponentInParent<IDamagable>();

        if (damageable != null)
        {
            hitList.Add(other);
            damageable.TakeDamage(damage, attackPlayer);

            //Knock Back
            NavMeshAgent agent = other.gameObject.GetComponentInParent<NavMeshAgent>();
            if (agent != null)
            {
                Debug.Log("1");
                agent.enabled = false;
                Vector3 direction = (other.transform.position - transform.position).normalized;
                direction.y = 0;//set y axis dont effect
                other.transform.parent.position += direction * KnockBackPower;
                agent.enabled = true;
            }

            if (impactEffect != null)
            {
                float offset = 0.7f;
                Vector3 newPosition = transform.position - transform.forward * offset;

                Instantiate(impactEffect, newPosition, transform.rotation);
            }

            if (remainingPierces > 0)
            {
                remainingPierces--;
                return;
            }
        }

        if (hasHit) return;
        hasHit = true;
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (returnToPool != null)
        {
            returnToPool(this);//Send this bullet back to the pool that created it.
        }
        else
        {
            gameObject.SetActive(false);//a safety fallback, just in case it is not connected to the pool, just disable it
        }
    }
}