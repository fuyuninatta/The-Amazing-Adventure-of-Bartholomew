using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public Slider healthSlider, easehealthSlider, containerhealthSlider;
    public Text healthText, healingPotionsText, ammo1Text, ammo2Text, ammo3Text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
