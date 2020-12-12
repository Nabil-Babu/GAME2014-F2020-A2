using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimStates
{
    IDLE,
    RUNNING, 
    JUMPING,
    DOUBLEJUMPING,
    ATTACKING,
}
public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    public Joystick joystick;
    public float joystickHorizontalSensitivity;
    public float horizontalForce;
    public float verticalForce;
    private Animator m_animator;
    private Rigidbody2D m_rigidBody2D;

    [Header("Player State")]
    [SerializeField]
    private bool isGrounded = false;
    private bool isDoubleJumping = false;  
    public PlayerAnimStates currentState; 
    [Header("Player Abilities")] 
    public int health;
    public int lives;

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
        if(joystick.Horizontal > joystickHorizontalSensitivity || Input.GetKey(KeyCode.D))
        {
            // Move Player Right
            m_rigidBody2D.AddForce(Vector2.right * horizontalForce * Time.deltaTime);
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


        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump(); 
        }
    }

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
        else if(!isGrounded)
        {
            if(!isDoubleJumping)
            {
                isDoubleJumping = true;
                m_rigidBody2D.AddForce(Vector2.up * verticalForce);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        isGrounded = true;
        isDoubleJumping = false; 
    }
}
