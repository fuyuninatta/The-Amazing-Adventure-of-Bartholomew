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

    //Health Bar & Easing
    public float lerpSpeed = 5f;
    public float widthPerHP = 2.5f;

    //sfx
    public AudioClip healsfx, gethitsfx, deathsfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {

        if (PlayerPrefs.HasKey("healingPotionAmount"))
        {
            healPotion = PlayerPrefs.GetInt("healingPotionAmount");
        }

        if(PlayerPrefs.HasKey("maxHealth"))
        {
            maxHealth = PlayerPrefs.GetInt("maxHealth");
        }

        if (PlayerPrefs.HasKey("currentHealth"))
        {
            currentHealth = PlayerPrefs.GetInt("currentHealth");
        }

        UpdateHealthUI();
        UIController.instance.easehealthSlider.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(invincibleCounter>0)
        {
            invincibleCounter -= Time.deltaTime;
        }
        if(damageOverlay != null && damageOverlay.color.a > 0)
        {
            Color overlayColor = damageOverlay.color;
            overlayColor.a -= flashSpeed * Time.deltaTime;
            damageOverlay.color = overlayColor;
        }

        //make ease health bar slightly slower
        if (UIController.instance.healthSlider.value != UIController.instance.easehealthSlider.value)
        {
            UIController.instance.easehealthSlider.value = Mathf.Lerp(UIController.instance.easehealthSlider.value, currentHealth, lerpSpeed * Time.deltaTime);
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
        UpdateHealthUI();
    }

    public void healPlayer()
    {
        currentHealth += healAmount;
        PlayerController.instance.audiosource.PlayOneShot(healsfx, 0.5f);
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthUI();
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
                PlayerController.instance.audiosource.PlayOneShot(deathsfx, 1.0f);
                PlayerController.instance.SaveGunData();
                transform.parent.gameObject.SetActive(false);
                currentHealth = 0;
                updateHealth();
                GameManager.instance.PlayerDied();
            }

            invincibleCounter = invincibleLength;
            UpdateHealthUI();
        }
    }

    public void IncreaseMaxHealth()
    {
        maxHealth += AddMaxHealthAmount;
        currentHealth += AddMaxHealthAmount;

        if (currentHealth > maxHealth)//prevent error
        {
            currentHealth = maxHealth;
        }
        UpdateHealthUI();
    }

    public void updateHealth()
    {
        PlayerPrefs.SetInt("healingPotionAmount", healPotion);
        PlayerPrefs.SetInt("maxHealth", maxHealth);
        PlayerPrefs.SetInt("currentHealth", currentHealth);
    }

    public void UpdateHealthUI()
    {
        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.easehealthSlider.maxValue = maxHealth;
        UIController.instance.containerhealthSlider.maxValue = maxHealth;

        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.containerhealthSlider.value = maxHealth;

        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
        UIController.instance.healingPotionsText.text = "X" + healPotion;

        float scaleFactor = (float)maxHealth / 100f;
        if (scaleFactor < 1f) scaleFactor = 1f;

        Vector3 scale = UIController.instance.SliderScale.localScale;
        scale.x = scaleFactor;
        UIController.instance.SliderScale.localScale = scale;
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
