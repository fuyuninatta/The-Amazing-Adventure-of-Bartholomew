using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public Slider healthSlider, easehealthSlider, containerhealthSlider;
    public RectTransform SliderScale;
    public Text healthText, healingPotionsText;

    public Text timerText;
    public float timeElapsed = 0f;

    public GameObject[] weaponBoxes;
    public Text[] ammoText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (PlayerPrefs.HasKey("Timer"))
        {
            timeElapsed = PlayerPrefs.GetFloat("Timer");
        }

        UpdateTimerDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name != "StartScene" && SceneManager.GetActiveScene().name != "LastScene")
        {
            timeElapsed += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
