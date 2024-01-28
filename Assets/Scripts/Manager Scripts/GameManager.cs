using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    [SerializeField] PlayerController playerController;
    
    // Score of the player
    [HideInInspector]
    public int score;

    // The amount filled in the hunger meter
    private float satiate;

    [Header("Variables")]
    [Tooltip("The maximum amount of fill in the hunger meter. Satiate decreases by 1/second by default. Defaults to 100 in code if left at 0.")]
    public float maxSatiate;

    [Tooltip("EXPOSED ONLY FOR TESTING. How fast the hunger meter decreases in x/second. Defaults to 1 in code if left at 0.")]
    public int hungerSpeed;

    public enum GameState
    {
        MENU,
        PLAYING,
        PAUSED,
        ENDED
    }

    [HideInInspector]
    public GameState currentState;

    private float timeFromStart;

    [Header("Intervals")]
    [Tooltip("The first interval when the hunger speed rises. Amount is in seconds after the game started. Defaults to 60 in code if left at 0.")]
    public int firstInterval;

    [Tooltip("The second interval when the hunger speed rises even further. Amount is in seconds after the first interval. Defaults to 60 in code if left at 0.")]
    public int secondInterval;

    public PlayerRopeWind ropeWindScript;

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        manager = null;
    }
#endif

    void Start()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        score = 0;
        timeFromStart = 0;

        // If user didn't specify the values, give them a default value.
        if(maxSatiate <= 0)
        {
            maxSatiate = 100;
        }
        if(hungerSpeed <= 0)
        {
            hungerSpeed = 1;
        }
        if(firstInterval == 0)
        {
            firstInterval = 60;
        }
        if(secondInterval == 0)
        {
            secondInterval = 60;
        }

        satiate = maxSatiate / 10;
    }

    private void Update()
    {
        if(currentState != GameState.PLAYING)
        {
            return;
        }

        if(satiate < 0)
        {
            // Lose game
            EndGame();
        }

        // Decrease satiate with each frame
        satiate -= Time.deltaTime * hungerSpeed;

        timeFromStart += Time.deltaTime;

        // Multiplies the speed at which hunger meter decreases if intervals have been met
        if(timeFromStart >= firstInterval)
        {
            satiate -= Time.deltaTime * hungerSpeed;

            if (timeFromStart >= firstInterval + secondInterval)
            {
                satiate -= Time.deltaTime * hungerSpeed;
            }
        }

        UpdateHungerUI();
    }

    public GameManager GetGameManager()
    {
        return manager;
    }

    public void AddScore(int scoreWorth)
    {
        score += scoreWorth;
        AddSatiate();
        UpdateScoreUI();
    }

    public void ReduceScore(int scoreWorth)
    {
        score -= scoreWorth;
        UpdateScoreUI();
        ReduceSatiate();
    }

    public int GetScore()
    {
        return score;
    }

    private void AddSatiate()
    {
        if (satiate + 10 >= maxSatiate)
        {
            satiate = maxSatiate;
            return;
        }

        satiate += 10;
        UpdateHungerUI();
    }

    private void ReduceSatiate()
    {
        satiate -= 10;
        UpdateHungerUI();
    }

    public void StartGame()
    {
        // T�h�n playaamaan se helikopterin alku animaatio tms
        playerController.StartCameraBlend();
        ropeWindScript.DetatchRope();
    }

    public void StartPlaying()
    {
        // T�� runnataan kun helikopteri juttu loppuu. Aika yms alkaa kulkemaan vasta kun pelaajasta saa kontrollin
        currentState = GameState.PLAYING;
        timeFromStart = 0;
    }

    public void PauseGame()
    {
        currentState = GameState.PAUSED;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Open pause menu..? Or is opened somewhere else?
    }

    public void UnPauseGame()
    {
        currentState = GameState.PLAYING;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // Close pause menu or done somewhere else?
    }

    public void EndGame()
    {
        currentState = GameState.ENDED;
        MenuManager.instance.OpenGameOverMenu();
        
        Time.timeScale = 0;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Open death screen and then main menu?
    }

    void UpdateScoreUI()
    {
        MenuManager.instance.gameplayUI.scoreText.text = score.ToString();
    }

    void UpdateHungerUI()
    {
        MenuManager.instance.gameplayUI.hungerMeter.fillAmount = satiate / maxSatiate;
        playerController.OnFatnessChanged(Mathf.Clamp((score / 200), 0, 1));
    }
}
