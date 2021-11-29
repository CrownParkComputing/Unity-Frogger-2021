using System.Collections;
using UnityEngine;


public class Frogger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public Sprite idleSprite;
    public Sprite leapSprite;
    public Sprite deathSprite;

    public AudioClip leapSound;
    public AudioClip leapSquash;
    public AudioClip leapSplash;
    private AudioSource playSound;
    public float volume = 0.5f;

    public Vector3 spawnPosition;
    private float farthestRow;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playSound = GetComponent<AudioSource>();
        spawnPosition = transform.position;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Move(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            Move(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            Move(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            Move(Vector3.right);
        }
    }

    private void Move(Vector3 direction)
    {
        Vector3 destination = transform.position + direction;

        Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));
        Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));
        Collider2D water = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Water"));

        if (barrier != null)
        {
            return;
        }

        if (platform != null)
        {
            transform.SetParent(platform.transform);
        } else
        {
            transform.SetParent(null);
        }

        if (water != null && platform == null)
        {
            playSound.PlayOneShot(leapSplash);
            transform.position = destination;
            Death();
        }
        
        if (obstacle != null && platform == null)
        {
            playSound.PlayOneShot(leapSquash);
            transform.position = destination;
            Death();
        }
        else
        {
            if (destination.y > farthestRow)
            {
                farthestRow = destination.y;
                FindObjectOfType<GameManager>().AdvanceScore();
            }

            StopAllCoroutines();
            
            StartCoroutine(Leap(destination));
        }
        
    }

    private IEnumerator Leap(Vector3 destination)
    {
        
        spriteRenderer.sprite = leapSprite;
        Vector3 startPosition = transform.position;

        float elapsed = 0f;
        float duration = 0.125f;
        playSound.PlayOneShot(leapSound);
        while (elapsed < duration)
        {
           
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = destination;
        spriteRenderer.sprite = idleSprite;
    }

    public void Death()
    {
        StopAllCoroutines();
        enabled = false;


        transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = deathSprite;
       
        Invoke(nameof(Respawn), 1f);

        FindObjectOfType<GameManager>().Died();
    }

    public void Respawn()
    {
        StopAllCoroutines();
        transform.rotation = Quaternion.identity;
        transform.position = spawnPosition;
        farthestRow = spawnPosition.y;
        spriteRenderer.sprite = idleSprite;
        gameObject.SetActive(true);
        enabled = true;      
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool hitObstacle = collision.gameObject.layer == LayerMask.NameToLayer("Obstacle");
        bool onPlatform = transform.parent != null;

        if (enabled && hitObstacle && !onPlatform)
        {
            Death();
        }
    }


}
