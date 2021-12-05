using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    private Frogger frogger;
    private Home[] homes;

    private int score;
    private int highScore;
    private int lives;
    private int time;

    public Slider slider;
    public Text scoreText;
    public Text highScoreText;
    public GameObject startPage;
    public GameObject playGamePage;
    public GameObject gameOverPage;
    enum PageState
    {
        Start,
        GameOver,
        Game
    }

    public AudioClip startGame;
    private AudioSource playSound;
    public float volume = 0.5f;

    bool gameOver = true;

    public bool GameOver { get { return gameOver; } }
    public int Score { get { return score; } }



    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        homes = FindObjectsOfType<Home>();    
        frogger = FindObjectOfType<Frogger>();
        playSound = GetComponent<AudioSource>();
        SetPageState(PageState.Start);
    }

    public void StartGame()
    {
        NewGame();
    }
    private void NewGame()
    {
        SetPageState(PageState.Game);
        playSound.PlayOneShot(startGame, volume);
        SetScore(0);
        SetLives(3);
        NewLevel();

    }

    private void NewLevel()
    {
        
        for (int i = 0; i < homes.Length; i++)
        {
            homes[i].enabled = false;   
        }
        Respawn();
    }

    private void Respawn()
    {
        frogger.Respawn();
        StopAllCoroutines();
        StartCoroutine(Timer(30));
    }

    private IEnumerator Timer(int duration)
    {
        slider.value = duration;
        time = duration;

        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
            slider.value = time;
        }

        frogger.Death();
    }

    public void Died()
    {
        SetLives(lives - 1);

        if (lives > 0)
        {
            Invoke(nameof(Respawn), 1f);
        }
        else
        {
            Invoke(nameof(GameEnd), 1f);
        }
    }

    private void GameEnd()
    {
        SetPageState(PageState.GameOver);
        StopAllCoroutines();
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                playGamePage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                playGamePage.SetActive(false);
                break;
            case PageState.Game:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                playGamePage.SetActive(true);
                break;
        }
    }

    public void AdvanceScore()
    {
        SetScore(score + 10);

    }
    public void HomeOccupied()
    {
        int bonusPoints = time * 20;
        frogger.gameObject.SetActive(false);

        SetScore(score + bonusPoints + 50);

        if (Cleared())
        {
            SetScore(score + 1000);
            Invoke(nameof(NewLevel), 1f);
        }
        else
        {
            Invoke(nameof(Respawn), 1f);
        }
    }

    private bool Cleared()
    {
        for (int i = 0; i < homes.Length; i++)
        {
            if (!homes[i].enabled)
            {
                return false;
            }
        }
        return true;
    }


    private void SetScore (int score)
    {
        this.score = score;
        scoreText.text = "Score:" + score.ToString();

        if (this.score > this.highScore)
        {
            SetHighScore(this.score);
        }

    }

    private void SetHighScore(int highScore)
    { 
        this.highScore = highScore;
        highScoreText.text = "High Score:" + highScore.ToString();

    }

    private void SetLives (int lives)
    {
        this.lives = lives; 

    }
}

