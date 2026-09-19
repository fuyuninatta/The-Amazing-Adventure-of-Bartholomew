using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public bool Trigger = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerAnimation()
    {
        Trigger = true;
    }
}
