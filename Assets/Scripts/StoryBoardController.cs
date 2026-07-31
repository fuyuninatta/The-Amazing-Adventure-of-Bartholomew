using TMPro;
using UnityEngine;

public class StoryBoardController : MonoBehaviour
{
    private TextMeshPro text;
    public AudioClip Booksfx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        text.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            text.enabled = true;
            PlayerController.instance.audiosource.PlayOneShot(Booksfx,0.1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            text.enabled = false;
            PlayerController.instance.audiosource.PlayOneShot(Booksfx, 0.1f);
        }
    }
}
