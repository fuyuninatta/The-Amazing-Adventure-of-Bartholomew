using UnityEngine;
using UnityEngine.AI;

public class EnemyHealthController : MonoBehaviour, IDamagable
{
    public int currentHealth = 5;

    public AudioClip GetHitSfx, DeathSfx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage, bool attackPlayer)
    {
        if (!attackPlayer)
        {
            currentHealth -= damage;
            PlayerController.instance.audiosource.PlayOneShot(GetHitSfx,0.2f);
            if (currentHealth <= 0)
            {
                PlayerController.instance.audiosource.PlayOneShot(DeathSfx, 0.2f);
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
