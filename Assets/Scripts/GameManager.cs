using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Frogger frogger;

    private Home[] homes;

    private int score;
    private int lives;
    private int time;

    public AudioClip startGame;
    private AudioSource playSound;
    public float volume = 0.5f;



    private void Awake()
    {
        homes = FindObjectsOfType<Home>();    
        frogger = FindObjectOfType<Frogger>();
        playSound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        NewGame();
    }
    private void NewGame()
    {
        playSound.PlayOneShot(startGame);
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
        time = duration;

        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
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
            //Invoke(nameof(GameOver), 1f);
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
    }

    private void SetLives (int lives)
    {
        this.lives = lives; 

    }
}

