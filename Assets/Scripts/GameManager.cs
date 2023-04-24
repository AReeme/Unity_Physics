using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameWinUI;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] AudioSource gameMusic;
    [SerializeField] TextMeshProUGUI deathText;

    public enum State
    {
        START_GAME,
        PLAY_GAME,
        GAME_WIN
    }

    public State state = State.START_GAME;
    float stateTimer = 0;
    public float timer = 0;
    float initialTime = 0;

    private void Start()
    {
        initialTime = PlayerPrefs.GetFloat("SavedTime", 0);
        timer += initialTime;
    }

    public void Update()
    {
        switch (state)
        {
            case State.START_GAME:
                Cursor.lockState = CursorLockMode.Locked;
                gameMusic.Play();
                state = State.PLAY_GAME;
                break;
            case State.PLAY_GAME:
                timer += Time.deltaTime;
                timerText.SetText("Time:" + timer.ToString("0.00"));
                break;
            case State.GAME_WIN:
                timer = 0;
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    gameWinUI.SetActive(false);
                    state = State.START_GAME;
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
    }
}
