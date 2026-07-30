using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointController : MonoBehaviour
{
    public string cpName;
    public AudioClip checkpointAC;
    public GameObject checkpointEffect;

    public static GameObject activeEffectInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "_cp"))
        {
            if(PlayerPrefs.GetString(SceneManager.GetActiveScene().name + "_cp") == cpName)
            {
                PlayerController.instance.GetComponent<CharacterController>().enabled = false;
                PlayerController.instance.transform.position = transform.position + new Vector3(0.0f,5.0f,0.0f);
                PlayerController.instance.transform.rotation = transform.rotation;
                PlayerController.instance.GetComponent<CharacterController>().enabled = true;
                CheckPointVFX();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (PlayerPrefs.GetString(SceneManager.GetActiveScene().name + "_cp") == cpName)//if empty checkpoint just skip the code
            {
                return;
            }
            PlayerController.instance.audiosource.PlayOneShot(checkpointAC, 0.5f);
            CheckPointVFX();
            PlayerPrefs.SetString(SceneManager.GetActiveScene().name + "_cp", cpName);
        }
    }

    private void CheckPointVFX()
    {
        if (activeEffectInstance != null)//if scene alr have effect, move to new position
        {
            activeEffectInstance.transform.position = transform.position;
            activeEffectInstance.transform.rotation = transform.rotation;
        }
        else//if scene dont have effect just create new
        {
            activeEffectInstance = Instantiate(checkpointEffect, transform.position, transform.rotation);
        }
    }
}
