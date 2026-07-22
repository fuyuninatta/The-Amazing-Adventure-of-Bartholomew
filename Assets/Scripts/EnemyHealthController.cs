using UnityEngine;

public class EnemyHealthController : MonoBehaviour, IDamagable
{
    public int currentHealth = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageEnemy()
    { 
        currentHealth--;
        if (currentHealth <= 0)
        {
            if(transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else 
            {
                Destroy(gameObject);
            }    
        }
    }

    public void TakeDamage(int damage, bool attackPlayer)
    {
        if (!attackPlayer)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
