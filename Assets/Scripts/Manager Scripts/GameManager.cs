using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager manager;

    [HideInInspector]
    public int score;

    [HideInInspector]
    public int highScore;

    public enum GameState
    {

    }

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
        highScore = 0;
    }

    public GameManager GetGameManager()
    {
        return manager;
    }

    public void AddScore()
    {
        score++;
        highScore++;
    }

    public void ReduceScore()
    {
        score--;
    }


}
