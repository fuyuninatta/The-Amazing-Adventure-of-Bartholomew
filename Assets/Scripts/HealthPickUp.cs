using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    //public int HealthAmount;
    public bool HealingP = false;//if HealingP is true the item is Healing Potion, else it is potion to maximize maxHealth

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(HealingP)
            {
                PlayerHeathController.instance.healPotion++;
                UIController.instance.healingPotionsText.text = "Healing Potions: " + PlayerHeathController.instance.healPotion;
            }
            else
            {
                PlayerHeathController.instance.IncreaseMaxHealth();
            }
                Destroy(gameObject);
        }
    }
}
