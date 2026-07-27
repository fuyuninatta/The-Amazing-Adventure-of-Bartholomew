using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider healthSlider;
    public Slider easeHealthSlider;
    private EnemyHealthController enemyHealth;
    private float lerpSpeed = 5f;

    private int currentHealth, maxHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<EnemyHealthController>();
        }

        healthSlider.maxValue = enemyHealth.maxHealth;
        easeHealthSlider.maxValue = enemyHealth.maxHealth;

        healthSlider.value = enemyHealth.currentHealth;
        easeHealthSlider.value = enemyHealth.currentHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if (healthSlider.value != enemyHealth.currentHealth)
        {
            healthSlider.value = enemyHealth.currentHealth;
        }

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, enemyHealth.currentHealth, lerpSpeed * Time.deltaTime);
        }
    }
}
