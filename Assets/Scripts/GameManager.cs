using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float WaitAfterDying = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //change mouse cursor mode
        if (SceneManager.GetActiveScene().name != "LastScene" && SceneManager.GetActiveScene().name != "StartScene")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //remove last level data
        if(SceneManager.GetActiveScene().name == "StartScene")
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if(UIController.instance != null)
        {
            PlayerPrefs.SetFloat("Timer", UIController.instance.timeElapsed);
        }

        if(PlayerController.instance!=null)
        {
            PlayerPrefs.SetInt("currentHealth", PlayerHeathController.instance.currentHealth);
        }

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)//prevent error
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    public void PlayerDied()
    {
        StartCoroutine(PlayerDiedCo());
    }
    public IEnumerator PlayerDiedCo()
    {
        yield return new WaitForSeconds(WaitAfterDying);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}