using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private PlayerLife pl;

    [SerializeField] GameObject titleUI;
    [SerializeField] GameObject gameWinUI;
    [SerializeField] GameObject gameLoseUI;
    [SerializeField] GameObject pauseUI;
    [SerializeField] GameObject controlsUI;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] AudioSource gameMusic;
    [SerializeField] TextMeshProUGUI deathText;
    [SerializeField] TextMeshProUGUI totalDeathWinText;
    [SerializeField] TextMeshProUGUI totalTimeWinText;
    [SerializeField] TextMeshProUGUI totalDeathLoseText;
    [SerializeField] TextMeshProUGUI totalTimeLoseText;
    [SerializeField] AudioSource loseSound;

    public enum State
    {
        TITLE,
        START_GAME,
        PLAY_GAME,
        GAME_WIN,
        PAUSE_GAME,
        GAME_LOSE 
    }

    public State state = State.TITLE;
    float stateTimer = 0;
    public float timer = 0;
    float initialTime = 0;
    int deathCount = 0;

    private void Start()
    {
        pl = FindObjectOfType<PlayerLife>();
        timer = 0;
        deathCount = 0;
        timer += initialTime;
    }

    public void Update()
    {
        switch (state)
        {
            case State.TITLE:
                titleUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                gameMusic.Stop();
                loseSound.Stop();
                break;
            case State.START_GAME:
                Cursor.lockState = CursorLockMode.Locked;
                titleUI.SetActive(false);
                gameMusic.Play();
                state = State.PLAY_GAME;
                break;
            case State.PLAY_GAME:
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    SetPause();
                }
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    SetLose();
                }
                timer += Time.deltaTime;
                timerText.SetText("Time:" + timer.ToString("0.00"));
                break;
            case State.PAUSE_GAME:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
                pauseUI.SetActive(true);
                controlsUI.SetActive(false);
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    ResumeGame();
                }
                break;
            case State.GAME_WIN:
                timer = 0;
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    gameWinUI.SetActive(false);
                    state = State.TITLE;
                }
                break;
            case State.GAME_LOSE:
                timer = 0;
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    gameLoseUI.SetActive(false);
                    state = State.TITLE;
                }
                break;
            default:
                break;
        }
    }


    public void SetWin()
    {
        gameWinUI.SetActive(true);
        gameMusic.Stop();
        state = State.GAME_WIN;
        stateTimer = 3;

        // Show total time and death count in the win UI
        totalDeathWinText.SetText("Total Resets: " + deathCount);
        totalTimeWinText.SetText("Total Time: " + timer.ToString("0.00"));

        deathCount = 0; // Reset the death count
        deathText.SetText("Resets: 0");
        pl.Respawn();
    }

    public void SetLose()
    {
        gameLoseUI.SetActive(true);
        gameMusic.Stop();
        loseSound.Play();
        state = State.GAME_LOSE;
        stateTimer = 5;

        // Show total time and death count in the Lose UI
        totalDeathLoseText.SetText("Resets Before Quitting: " + deathCount);
        totalTimeLoseText.SetText("Time Before Quitting: " + timer.ToString("0.00"));

        deathCount = 0; // Reset the death count
        deathText.SetText("Resets: 0");
        pl.Respawn();
        
    }

    public void StartGame()
    {
        state = State.START_GAME;
    }

    public void IncrementDeathCount()
    {
        deathCount++;
        deathText.SetText("Resets: " + deathCount);
    }

    public void SetPause()
    {
        state = State.PAUSE_GAME;
    }

    public void ResumeGame()
    {
        state = State.PLAY_GAME;
        Time.timeScale = 1; // Resume time
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controlsUI.SetActive(true);
        pauseUI.SetActive(false);
    }

}
