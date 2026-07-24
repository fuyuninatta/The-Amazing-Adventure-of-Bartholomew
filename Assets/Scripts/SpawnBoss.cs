using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public GameObject Boss;
    public GameObject Gate;
    public GameObject BossHealthCanvas;
    public GameObject BGM1, BGM2;

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
            //set up boss, boss health, gate
            Boss.gameObject.SetActive(true);
            Gate.gameObject.SetActive(true);
            BossHealthCanvas.gameObject.SetActive(true);

            //CHANGE BGM
            BGM1.gameObject.SetActive(false);
            BGM2.gameObject.SetActive(true);
        }
    }
}
