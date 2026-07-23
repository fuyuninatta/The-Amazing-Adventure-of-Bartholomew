using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public GameObject Boss;
    public GameObject Gate;

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
            Boss.gameObject.SetActive(true);
            Gate.gameObject.SetActive(true);
        }
    }
}
