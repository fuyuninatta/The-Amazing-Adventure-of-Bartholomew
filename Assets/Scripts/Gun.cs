using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Bullet Pool")]
    public BulletController bulletPrefab;
    public int poolSize = 30;

    [Header("Gun Settings")]
    public bool canAutoFire;
    public float fireRate;
    [HideInInspector]
    public float fireCounter;

    [Header("Ammo")]
    public int currentAmmo;

    private Queue<BulletController> bulletPool = new Queue<BulletController>();
    private Transform poolParent;
    private bool poolReady;


    // Update is called once per frame
    void Update()
    {
        if (fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;
        }
    }
    public void PreparePool()//Create bullets in advance and store them in the bullet pool.
    {
        if (poolReady) return;//This checks whether the pool has already been created, if yes stop the code

        GameObject parentObj = new GameObject(gameObject.name + "_BulletPool"); //creates a new empty GameObject, based on gun name, e.g. Pistol_BulletPool
        poolParent = parentObj.transform;//This saves the transform into poolParent

        for (int i = 0; i < poolSize; i++)
        {
            BulletController bullet = Instantiate(bulletPrefab, poolParent);//This creates one bullet from your bullet prefab and placed under the pool object in the hierarchy
            bullet.gameObject.SetActive(false);
            bullet.SetReturnAction(ReturnBullet);//This tells the bullet how to return to this gun¡¯s pool.
            //Bullet, when you are finished, call this gun's ReturnBullet function. this will call returnToPool(this)--> will call ReturnBullet(bullet)
            //it will return the bullet to the correct pool
            bulletPool.Enqueue(bullet);//This puts the inactive bullet into the pool.
        }

        poolReady = true;
    }

    public BulletController GetBullet(Vector3 position, Quaternion rotation)//take one bullet from the pool
    {
        PreparePool();//a safety check, just in case not prepared yet

        BulletController bullet;

        if (bulletPool.Count > 0)//This checks how many inactive bullets are currently waiting in the pool.
        {
            bullet = bulletPool.Dequeue();//Pull one from the pool
        }
        else//if the pool is empty or no inactive bullets available
        {
            bullet = Instantiate(bulletPrefab, poolParent);//this creates one extra bullet
            bullet.SetReturnAction(ReturnBullet);//return to the correct pool when finished
        }

        bullet.transform.SetPositionAndRotation(position, rotation);//set the position n rotation correctly based on firepoint
        bullet.gameObject.SetActive(true);//activate it, make the function alive
        bullet.Fire();//move it forward

        return bullet;
    }

    public void ReturnBullet(BulletController bullet)
    {
        bullet.gameObject.SetActive(false);
        bulletPool.Enqueue(bullet);//put it back to the pool
    }
}