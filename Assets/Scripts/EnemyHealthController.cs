using UnityEngine;
using UnityEngine.AI;

public class EnemyHealthController : MonoBehaviour, IDamagable
{
    public int currentHealth = 5, maxHealth = 10;
    private HealthBar healthBar;

    public AudioClip GetHitSfx, DeathSfx;

    private EnemyController enemyController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        enemyController = GetComponent<EnemyController>();
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
            PlayerController.instance.audiosource.PlayOneShot(GetHitSfx,0.4f);

            //UI
            healthBar.healthSlider.value = currentHealth;

            //dead
            if (currentHealth <= 0)
            {
                PlayerController.instance.audiosource.PlayOneShot(DeathSfx, 0.6f);
                transform.GetComponent<EnemyController>().Dead();
            }
            else
            {
                //put get hit animation
                enemyController.GetHitAnim();
            }
        }
    }
}
