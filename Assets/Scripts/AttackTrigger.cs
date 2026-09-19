using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public bool Attack = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerAttack()
    {
        Attack = true;
    }
}
