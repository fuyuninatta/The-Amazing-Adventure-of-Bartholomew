using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    public int HealthAmount;

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
            PlayerHeathController.instance.healPlayer(HealthAmount);
            Destroy(gameObject);
        }
    }
}
