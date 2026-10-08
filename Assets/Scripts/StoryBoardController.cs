using TMPro;
using UnityEngine;

public class StoryBoardController : MonoBehaviour
{
    private TextMeshPro text;
    public GameObject Story;
    public AudioClip Booksfx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        text.enabled = false;
        Story.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Story.SetActive(true);
            PlayerController.instance.audiosource.PlayOneShot(Booksfx,0.1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Story.SetActive(false);
            PlayerController.instance.audiosource.PlayOneShot(Booksfx, 0.1f);
        }
    }
}
