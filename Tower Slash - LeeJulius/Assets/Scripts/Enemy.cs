using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum ArrowTypes
    {
        GREEN,
        RED,
        YELLOW
    }

    [Header("Arrow Sprites")]
    [SerializeField]
    private GameObject[] ArrowDirections;

    [Header("Enemy Properties")]
    public float moveSpeed;
    public ArrowTypes ArrowType;
    private int ArrowPosition;
    private SwipeDirections CorrectSwipe;

    private IEnumerator ArrowMovement;

    [Header("Enemy Conditions")]
    private bool inPlayerRange;
    private bool isAlive;
    [HideInInspector] public bool isDashed;

    // Start is called before the first frame update
    void Start()
    {
        inPlayerRange = false;
        isAlive = true;
        isDashed = false;

        SetEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
    }

    private void SetEnemy()
    {
        // Setting Starting Arrow Position
        ArrowPosition = Random.Range(0, ArrowDirections.Length);
        ArrowDirections[ArrowPosition].SetActive(true);

        // Setting Enemy Type
        int EnemyType = Random.Range(1, 4);

        // Applying Enemy Characteristics (Color of Arrow)
        switch (EnemyType)
        {
            // Green Enemy - Same Direction
            case 1:
                ArrowType = ArrowTypes.GREEN;
                ArrowDirections[ArrowPosition].GetComponent<SpriteRenderer>().color = Color.green;
                break;

            // Red Enemy - Opposite Direction
            case 2:
                ArrowType = ArrowTypes.RED;
                ArrowDirections[ArrowPosition].GetComponent<SpriteRenderer>().color = Color.red;
                break;

            // Yellow Enemy - Rotating Direction
            case 3:
                ArrowType = ArrowTypes.YELLOW;

                for (int i = 0; i < ArrowDirections.Length; i++)
                {
                    ArrowDirections[i].GetComponent<SpriteRenderer>().color = Color.yellow;
                }

                ArrowMovement = RotateArrow();
                StartCoroutine(ArrowMovement);
                break;

            default:
                Debug.LogWarning("No Avaiable Arrow Type");
                break;
        }
    }

    private void EnemyMovement()
    {
        // Move Enemy Down
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);

        // Destroy Self if out of screen
        if (transform.position.y < -10.0f)
        {
            DestroySelf();
        }
    }

    public void AttackEnemy(SwipeDirections PlayerSwipeDirection)
    {
        if (!inPlayerRange)
            return;

        Player CurrentPlayer = PlayerSelectionManager.instance.CurrentPlayer.GetComponent<Player>();

        // Checking if Player Swiped Correct Direction
        if (PlayerSwipeDirection == CorrectSwipe)
        {
            // Randomizing Powerup Chance
            int randomPowerUpChance = Random.Range(0, 100);

            // Add Extra Life if 3%
            if (randomPowerUpChance < 3)
            {
                CurrentPlayer.SetLives(CurrentPlayer.GetLives() + 1);
                GameManager.instance.UpdateLifeText();
            }

            // Add Dash
            CurrentPlayer.SetDash(CurrentPlayer.GetDash() + CurrentPlayer.GetDashIncrement());
            GameManager.instance.UpdateDashText();

            // Add Score
            CurrentPlayer.SetScore(CurrentPlayer.GetScore() + 1);
            GameManager.instance.UpdateScoreText();

            // Kill Enemy
            isAlive = false;
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        EnemySpawner.instance.SpawnedEnemies.Remove(this.gameObject);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Make Enemy Attackble when in Range with Player
        if (other.name.StartsWith("Player"))
        {
            this.GetComponent<SpriteRenderer>().color = Color.black;

            if (ArrowType == ArrowTypes.YELLOW)
                StopCoroutine(ArrowMovement);

            SetCorrectDirection(ArrowPosition, ArrowType);

            inPlayerRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name.StartsWith("Player"))
        {
            this.GetComponent<SpriteRenderer>().color = Color.white;
            inPlayerRange = false;

            Player CurrentPlayer = other.gameObject.GetComponent<Player>();

            // -1 HP if Enemy Passes Player (and not in power up)
            if (isAlive && !CurrentPlayer.isImmune)
            {
                CurrentPlayer.SetLives(CurrentPlayer.GetLives() - 1);
                GameManager.instance.UpdateLifeText();
            }
        }
    }

    private IEnumerator RotateArrow()
    {
        // Rotate Arrow while not in player range
        while (true)
        {
            Debug.Log("Arrow Position: " + ArrowPosition);

            ArrowDirections[ArrowPosition].SetActive(false);

            ArrowPosition++;

            if (ArrowPosition >= ArrowDirections.Length)
            {
                ArrowPosition = 0;
            }
                
            ArrowDirections[ArrowPosition].SetActive(true);

            yield return new WaitForSecondsRealtime(0.4f);
        }
    }

    private void SetCorrectDirection(int ArrowPosition, ArrowTypes ArrowType)
    {
        Debug.Log("Correct Position: " + ArrowPosition);

        switch (ArrowPosition)
        { 
            // If Arrow Position is Up
            case 0:
                if (ArrowType == ArrowTypes.GREEN || ArrowType == ArrowTypes.YELLOW)
                {
                    CorrectSwipe = SwipeDirections.UP;
                }
                else
                {
                    CorrectSwipe = SwipeDirections.DOWN;
                }
                
                break;

            // If Arrow Position is Left
            case 1:
                if (ArrowType == ArrowTypes.GREEN || ArrowType == ArrowTypes.YELLOW)
                {
                    CorrectSwipe = SwipeDirections.LEFT;
                }
                else
                {
                    CorrectSwipe = SwipeDirections.RIGHT;
                }
                break;

            // If Arrow Position is Down
            case 2:
                if (ArrowType == ArrowTypes.GREEN || ArrowType == ArrowTypes.YELLOW)
                {
                    CorrectSwipe = SwipeDirections.DOWN;
                }
                else
                {
                    CorrectSwipe = SwipeDirections.UP;
                }
                break;

            // If Arrow Position is Right
            case 3:
                if (ArrowType == ArrowTypes.GREEN || ArrowType == ArrowTypes.YELLOW)
                {
                    CorrectSwipe = SwipeDirections.RIGHT;
                }
                else
                {
                    CorrectSwipe = SwipeDirections.LEFT;
                }
                break;

            default:
                Debug.LogWarning("No Correct Swipe.");
                break;
        }
    }
}
