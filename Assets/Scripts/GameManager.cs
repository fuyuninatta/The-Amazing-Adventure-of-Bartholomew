using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

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
}