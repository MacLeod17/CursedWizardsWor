using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameSession : MonoBehaviour
{
    public int Score { get; set; } = 0;
    public int HighScore { get; set; } = 0;

    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI highScoreUI;
    public TextMeshProUGUI pipUI;
    //public TextMeshProUGUI livesUI;

    public GameObject gameOverScreen;
    public GameObject winGameScreen;

    GameObject player;
    Character character;
    List<GameObject> pips = null;
    public bool gameWon = false;

    static GameSession instance = null;
    public static GameSession Instance
    {
        get
        {
            return instance;
        }
    }

    public enum eState
    {
        Load,
        StartSession,
        Session,
        EndSession,
        GameOver,
        WinGame
    }

    public eState State { get; set; } = eState.Load;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        HighScore = (GameController.Instance != null) ? GameController.Instance.highScore : 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && GameController.Instance != null)
        {
            GameController.Instance.OnPause();
        }

        switch (State)
        {
            case eState.Load:
                Score = 0;
                if (highScoreUI != null) highScoreUI.text = string.Format("{0:D4}", HighScore);
                if (player == null)
                {
                    player = GameObject.FindGameObjectWithTag("Player");
                    character = player.GetComponent<Character>();
                }
                pips = new List<GameObject>(GameObject.FindGameObjectsWithTag("Pip"));
                State = eState.StartSession;
                break;
            case eState.StartSession:
                if (gameOverScreen != null) gameOverScreen.SetActive(false);
                if (winGameScreen != null) winGameScreen.SetActive(false);
                //GameController.Instance.transition.StartTransition(Color.clear, 1);
                State = eState.Session;
                break;
            case eState.Session:
                CheckDeath();
                break;
            case eState.EndSession:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (gameWon) State = eState.WinGame;
                else State = eState.GameOver;
                break;
            case eState.GameOver:
                if (gameOverScreen != null) gameOverScreen.SetActive(true);
                break;
            case eState.WinGame:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (winGameScreen != null) winGameScreen.SetActive(true);
                break;
            default:
                break;
        }        
    }

    public void AddPoints(int points)
    {
        Score += points;
        if (scoreUI != null) scoreUI.text = string.Format("{0:D4}", Score);

        SetHighScore();
    }

    public void QuitToMainMenu()
    {
        if (GameController.Instance != null) GameController.Instance.OnLoadMenuScene("MainMenu");
    }

    private void SetHighScore()
    {
        if (Score > HighScore)
        {
            HighScore = Score;
            if (GameController.Instance != null) GameController.Instance.SetHighScore(HighScore);
            if (highScoreUI != null) highScoreUI.text = string.Format("{0:D4}", HighScore);
        }
    }

    private void CheckDeath()
    {
        if (character.isDead)
        {
            State = eState.EndSession;
            player.SetActive(false);
            player = null;
            character = null;
        }
    }

    public void UpdatePips(GameObject pip)
    {
        pips.Remove(pip);
        if (pipUI != null) pipUI.text = string.Format("{0:D3}", pips.Count);
        if (pips.Count <= 0)
        {
            gameWon = true;
            State = eState.EndSession;
        }
    }
}
