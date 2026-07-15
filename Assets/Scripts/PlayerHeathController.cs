using UnityEngine;

public class PlayerHeathController : MonoBehaviour,IDamagable
{
    public static PlayerHeathController instance;
    public int maxHealth, currentHealth;
    public float invincibleLength = 1f;
    public float invincibleCounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        currentHealth = maxHealth;
        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if(invincibleCounter>0)
        {
            invincibleCounter -= Time.deltaTime;
        }
    }
    public void DamagePlayer(float damageAmount)
    {
        int damage = (int)(damageAmount);
        currentHealth -= damage;
        if(currentHealth<=0)
        {
            transform.parent.gameObject.SetActive(false);
            currentHealth = 0;
            GameManager.instance.PlayerDied();
        }
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }

    public void healPlayer(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }

    public void TakeDamage(int damage, bool attackPlayer)
    {
        if (invincibleCounter <= 0)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                transform.parent.gameObject.SetActive(false);
                currentHealth = 0;
                GameManager.instance.PlayerDied();
            }

            invincibleCounter = invincibleLength;

            UIController.instance.healthSlider.value = currentHealth;
            UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
        }
    }
}
