using UnityEngine;
using UnityEngine.AI;

public class EnemyHealthController : MonoBehaviour, IDamagable
{
    public int currentHealth = 5, maxHealth = 10;
    private HealthBar healthBar;

    public AudioClip GetHitSfx, DeathSfx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthBar = gameObject.GetComponentInChildren<HealthBar>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage, bool attackPlayer)
    {
        if (!attackPlayer)
        {
            //reduce health
            currentHealth -= damage;
            PlayerController.instance.audiosource.PlayOneShot(GetHitSfx,0.2f);

            //UI
            healthBar.healthSlider.value = currentHealth;

            //dead
            if (currentHealth <= 0)
            {
                PlayerController.instance.audiosource.PlayOneShot(DeathSfx, 0.2f);
                if (transform.parent != null)
                {
                    transform.parent.GetComponent<EnemyController>().Dead();
                }
            }
        }
    }
}
