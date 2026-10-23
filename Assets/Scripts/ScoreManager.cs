/*
 * Handles main gameplay scoring systems
 */


using TMPro;
using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreCounter;
    public int score;
    private GameObject[] pellets;
    public bool paused = false;
    public GameObject pauseScreen, winScreen, loseScreen, finalScore;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        score = 0;
        pellets = GameObject.FindGameObjectsWithTag("Pellet"); 
        Time.timeScale = 1;
        finalScore.SetActive(false);
    }

    void Update()
    {
        scoreCounter.text = score.ToString();
        if (score == pellets.Length)
        {//If player collects all pellets, they win
            Win();
        }
    }

    public void Win()
    { //Prepares win screen
        Pause(true);
        Debug.Log("You Win!");
        ShowScreen(winScreen);
        FinalScore();
    }

    public void Lose()
    { //prepares lose screen
        Pause(true);
        Debug.Log("You Lose");
        ShowScreen(loseScreen);
        FinalScore();
    }

    public void Pause(bool force = false)
    { //Pauses the game (Can be called by keyboard or by win/loss case
        if (!paused || force)
        {//Pauses
            Time.timeScale = 0;
            paused = true;
            Cursor.lockState = CursorLockMode.None;
            if (!force)
            {//Only shows screen if paused by player
                ShowScreen(pauseScreen);
            }
        }
        else
        {//Resumes
            Time.timeScale = 1;
            paused = false;
            Cursor.lockState = CursorLockMode.Locked;
            ShowScreen();
        }
    }

    private void FinalScore()
    { //Sets final score UI
        finalScore.SetActive(true);
        finalScore.GetComponent<TextMeshProUGUI>().text = $"Final Score - <color=\"Yellow\">{score}</color>";
    }

    public void ShowScreen(GameObject screen = null)
    { //Toggles all screens but 1 off (Can be called with no screen to turn all screens off)
        if (screen != null)
        {
            pauseScreen.SetActive(pauseScreen == screen);
            winScreen.SetActive(winScreen == screen);
            loseScreen.SetActive(loseScreen == screen);
        }
        else
        {
            pauseScreen.SetActive(false);
            winScreen.SetActive(false);
            loseScreen.SetActive(false);
        }
    }
}
