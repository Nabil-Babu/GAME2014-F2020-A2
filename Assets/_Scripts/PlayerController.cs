/**
    PlayerController.cs
    Nabil Babu
    101214336
    Dec 11th 2020
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public enum PlayerAnimStates
{
    IDLE,
    RUNNING, 
    JUMPING,
    DOUBLEJUMPING,
    HIT, 
    ATTACK,
}
public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    public Joystick joystick;
    public float joystickHorizontalSensitivity;
    public float horizontalForce;
    public float verticalForce;
   

    [Header("Player State")]
    [SerializeField]
    private bool isGrounded = false;
    private bool isDoubleJumping = false;  
    private bool isAttacking = false;  
    public PlayerAnimStates currentState; 
    [Header("Player Abilities")] 
    public float health;
    public float maxHealth; 
    public int playerScore = 0; 
    public GameObject healthBar;
    public GameObject attackPoint;
    public ContactFilter2D filter2D;
    private Animator m_animator;
    private Rigidbody2D m_rigidBody2D;
    public Text score; 
    public GameObject EndTitle; 
    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _Move();
    }

    void _Move()
    {
        if(!isAttacking)
        {
            if(joystick.Horizontal > joystickHorizontalSensitivity || Input.GetKey(KeyCode.D))
            {
                // Move Player Right
                m_rigidBody2D.AddForce(Vector2.right * horizontalForce * Time.deltaTime);
                // Flip Player 
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                if(isGrounded)
                {
                    currentState = PlayerAnimStates.RUNNING;
                    m_animator.SetInteger("AnimState", (int)currentState);
                }
            }
            else if (joystick.Horizontal < - joystickHorizontalSensitivity || Input.GetKey(KeyCode.A))
            {
                // Move Player Left
                m_rigidBody2D.AddForce(Vector2.left * horizontalForce * Time.deltaTime);
                // Flip Player 
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                if(isGrounded)
                {
                    currentState = PlayerAnimStates.RUNNING;
                    m_animator.SetInteger("AnimState", (int)currentState);
                }
            }
            else 
            {
                if(isGrounded)
                {
                    currentState = PlayerAnimStates.IDLE;
                    m_animator.SetInteger("AnimState", (int)currentState);
                }
            }
        }


        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump(); 
        }
    }

    // Players Jump function adds vertical force if grounded and enabled Double Jump if already jumping
    public void Jump()
    {
        if(isGrounded)
        {
            m_rigidBody2D.AddForce(Vector2.up * verticalForce);
            isGrounded = false;
            // Animate Jump
            currentState = PlayerAnimStates.JUMPING;
            m_animator.SetInteger("AnimState", (int)currentState);
        } 
        else if(!isGrounded) // If you're already jumping jump again
        {
            if(!isDoubleJumping) // stops triple jumping
            {
                isDoubleJumping = true;
                m_rigidBody2D.AddForce(Vector2.up * verticalForce);
            }
        }
    }

    // Fire the Attack Coroutine
    public void OnAttack()
    {
        if(!isAttacking) // Locks the coroutine
        {
            StartCoroutine(Attack());
        }
        
    }
    // Attack Coroutine, This CircleCasts to check for enemies in range and damages them. 
    IEnumerator Attack()
    {
        currentState = PlayerAnimStates.ATTACK;
        m_animator.SetInteger("AnimState", (int)currentState);
        isAttacking = true; // Flag to make sure player can't spam attack
        yield return new WaitForSeconds(0.33f);
        List<RaycastHit2D> results = new List<RaycastHit2D>(); // Holds results of the cast
        var hit = Physics2D.CircleCast(attackPoint.transform.position, 1.0f, new Vector2(0, 0), filter2D, results);
        if(hit > 0)
        {
            if(results[0].collider.gameObject.TryGetComponent<SkellyBoiBehaviour>(out SkellyBoiBehaviour skellyBoi)) // Checks to see if other object is a Skeleton
            {
                SoundManager.instance.PLaySE("Sword2");
                skellyBoi.TakeHit(new Vector2(transform.localScale.x, 0)); //Pass in the direct the hit is coming from 
            }
        } 
        yield return new WaitForSeconds(0.33f);
        isAttacking = false; 
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Coin"))
        {
            playerScore+=10; 
            UpdateScoreText(); // Updates the Text on the Canvas
            SoundManager.instance.PLaySE("Coin");
            Destroy(other.collider.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        isGrounded = true;
        isDoubleJumping = false;

        if(other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(other.gameObject.transform); //Makes sure the player is moving with the platform
        } 
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null); 
        } 
    }

    // Take Damage reduces health, plays animation, and updates healthbar
    public void TakeDamage(float damage, Vector2 direction)
    {
        health -= damage;
        currentState = PlayerAnimStates.HIT;
        m_animator.SetInteger("AnimState", (int)currentState);
        healthBar.transform.localScale = new Vector3(health/maxHealth, 1, 1);
        m_rigidBody2D.AddForce(direction * 1000);
        if(health <= 0)
        {
            health = 0;
            OnDeath(); // Triggers OnDeath Coroutine
        } 
    }

    public void UpdateScoreText()
    {
        score.text = playerScore.ToString();
    }

    public void AddToScore(int amount)
    {
        playerScore += amount;
        UpdateScoreText(); 
    }

    public void OnDeath()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        // Play Death Animation
        m_animator.SetTrigger("Death"); 
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("GameOverScene");  
    }

    // Starts End Game Coroutine
    public void OnEndGame()
    {
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        // Enabled End Title Text
        EndTitle.SetActive(true); 
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("GameOverScene");  
    }
}
