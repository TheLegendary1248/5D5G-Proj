using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    const int maxLevels = 15;
    static int currentLevel;
    static bool isPaused = false;
    static GameObject self;
    static bool hasLoaded = false;
    public void Start()
    {
        if (!hasLoaded)
        {
            self = gameObject;
            DontDestroyOnLoad(gameObject);
            self.SetActive(false);
        }
        else
        {
            hasLoaded = true;
            Destroy(gameObject);
        }
    }
    public static void LoadGame(int levelNumber)
    {
        if(levelNumber > maxLevels) { Debug.LogError("NonExistent Scene"); return; }
        currentLevel = levelNumber;
        if(self)self.SetActive(false);
        SceneManager.LoadScene($"Maze{levelNumber}");
        Time.timeScale = 1f;
    }
    public static void LoadNextGame()
    {
        SceneManager.LoadScene($"Maze{++currentLevel}");
        if(self)self.SetActive(false);
        Time.timeScale = 1f;
    }
    public static void Pause()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            self.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            self.SetActive(true);
        }
    }
}
