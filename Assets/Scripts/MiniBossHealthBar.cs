using UnityEngine;
using UnityEngine.UI;

public class MiniBossHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    private MiniBossController enemyHealth;
    private float lerpSpeed = 5f;

    private int currentHealth, maxHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<MiniBossController>();
        }

        healthSlider.maxValue = enemyHealth.MaxHealth;
        easeHealthSlider.maxValue = enemyHealth.MaxHealth;

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
