using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Transform cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
