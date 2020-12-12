using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    public Joystick joystick;
    public float joystickHorizontalSensitivity;
    public float horizontalForce;
    private Animator m_animator;
    private Rigidbody2D m_rigidBody2D;

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
        if(joystick.Horizontal > joystickHorizontalSensitivity)
        {
            // Move Player Right
            m_rigidBody2D.AddForce(Vector2.right * horizontalForce * Time.deltaTime);
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_animator.SetInteger("runState", 1);
        }
        else if (joystick.Horizontal < - joystickHorizontalSensitivity)
        {
            // Move Player Left
            m_rigidBody2D.AddForce(Vector2.left * horizontalForce * Time.deltaTime);
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            m_animator.SetInteger("runState", 1);
        }
        else 
        {
            m_animator.SetInteger("runState", 0);
        }
    }
}
