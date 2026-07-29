using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public static HealthBarUI instance;

    [Header("UI References")]
    [SerializeField] private Image mainHealthBar;
    [SerializeField] private Image easeHealthBar;

    [Header("Settings")]
    [SerializeField] private float lerpSpeed = 5f;

    private float targetFillAmount = 1f;

    private void Awake()
    {
        instance = this;
    }

    // Call this function from PlayerHeathController whenever health changes
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        // (float) cast prevents 20/100 returning 0
        targetFillAmount = (float)currentHealth / maxHealth;

        // Main red bar updates instantly
        mainHealthBar.fillAmount = targetFillAmount;

        // If healing, snap the ease bar instantly so it doesn't lag behind
        if (easeHealthBar.fillAmount < targetFillAmount)
        {
            easeHealthBar.fillAmount = targetFillAmount;
        }
    }

    private void Update()
    {
        // Smoothly shrinks the ease bar down to match the main bar when damaged
        if (easeHealthBar != null && mainHealthBar != null)
        {
            if (easeHealthBar.fillAmount > mainHealthBar.fillAmount)
            {
                easeHealthBar.fillAmount = Mathf.Lerp(easeHealthBar.fillAmount, mainHealthBar.fillAmount, Time.deltaTime * lerpSpeed);
            }
        }
    }
}