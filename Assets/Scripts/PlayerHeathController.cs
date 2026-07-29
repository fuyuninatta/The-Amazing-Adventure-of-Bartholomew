using UnityEngine;
using UnityEngine.UI;

public class PlayerHeathController : MonoBehaviour,IDamagable
{
    public static PlayerHeathController instance;
    public int maxHealth = 100, currentHealth, AddMaxHealthAmount = 15;
    public float invincibleLength = 1f;
    public float invincibleCounter;
    public int healAmount = 10, healPotion = 0;

    //Damage Overlay
    public Image damageOverlay;
    public float flashSpeed = 2f; //yang handle how fast it fade away

    //sfx
    public AudioClip gethitsfx, deathsfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        currentHealth = maxHealth;

        if (PlayerPrefs.HasKey("healingPotionAmount"))
        {
            healPotion = PlayerPrefs.GetInt("healingPotionAmount");
        }

        if (PlayerPrefs.HasKey("maxHealth"))
        {
            maxHealth = PlayerPrefs.GetInt("maxHealth");
        }

        if (HealthBarUI.instance != null)
        {
            HealthBarUI.instance.UpdateHealth(currentHealth, maxHealth);
        }

        // Update is called once per frame
        void Update()
        {
            if (invincibleCounter > 0)
            {
                invincibleCounter -= Time.deltaTime;
            }

            if (damageOverlay != null && damageOverlay.color.a > 0)
            {
                Color overlayColor = damageOverlay.color;
                overlayColor.a -= flashSpeed * Time.deltaTime;
                damageOverlay.color = overlayColor;
            }
        }
    }

    public void DamagePlayer(float damageAmount)
    {
        int damage = (int)(damageAmount);
        currentHealth -= damage;

        //get hit sfx
        PlayerController.instance.audiosource.PlayOneShot(gethitsfx, 0.5f);
        FlashRedScreen();

        if(currentHealth<=0)
        {
            //death sfx
            PlayerController.instance.audiosource.PlayOneShot(deathsfx, 1.0f);
            PlayerController.instance.SaveGunData();
            transform.parent.gameObject.SetActive(false);
            currentHealth = 0;
            GameManager.instance.PlayerDied();
        }

        HealthBarUI.instance.UpdateHealth(currentHealth, maxHealth);

        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }

    public void healPlayer()
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        HealthBarUI.instance.UpdateHealth(currentHealth, maxHealth);

        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }

    public void TakeDamage(int damage, bool attackPlayer)
    {
        if(!attackPlayer)
        {
            return;
        }
        if (invincibleCounter <= 0)
        {
            currentHealth -= damage;
            FlashRedScreen();

            if (currentHealth <= 0)
            {
                transform.parent.gameObject.SetActive(false);
                currentHealth = 0;
                updateHealth();
                GameManager.instance.PlayerDied();
            }

            invincibleCounter = invincibleLength;

            HealthBarUI.instance.UpdateHealth(currentHealth, maxHealth);

            UIController.instance.healthSlider.value = currentHealth;
            UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
        }
    }

    public void IncreaseMaxHealth()
    {
        maxHealth += AddMaxHealthAmount;
        currentHealth = maxHealth;
        HealthBarUI.instance.UpdateHealth(currentHealth, maxHealth);
        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }
    public void updateHealth()
    {
        PlayerPrefs.SetInt("healingPotionsAmount", healPotion);
        PlayerPrefs.SetInt("maxHealth", maxHealth);    
    }

    private void FlashRedScreen()
    {
        if (damageOverlay != null)
        {
            Color overlayColor = damageOverlay.color;
            overlayColor.a = 0.9f; //opacity
            damageOverlay.color = overlayColor;
        }
    }
}
