/**
    SkellyBoiBehaviour.cs
    Nabil Babu
    101214336
    Dec 13th 2020
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AnimStates{
    IDLE,
    WALKING,
    ATTACK,
    HIT,
    DEATH
}

public enum AIStates{
    PATROL,
    CHASE,
    ATTACKING
}
public class SkellyBoiBehaviour : MonoBehaviour
{
    private Animator m_Animator; 
    private Rigidbody2D m_Rigidbody;
    private GameObject target; 

    [Header("SkellyBoi Variables")]
    public GameObject groundSensor;
    public GameObject wallSensor;
    public LayerMask collisionWallLayer;
    public ContactFilter2D filter;
    public float sightRange;
    public float attackRange;
    public bool groundAhead = true;
    public bool wallAhead = false; 
    public bool playerInSight = false; 
    public bool isAttacking = false;
    public bool takingHit = false;
    public bool hitPlayer = false; 
    public AnimStates currentAnimState; 
    public AIStates currentAIState; 
    public float walkSpeed; 
    public float chaseSpeed; 
    public Vector2 direction; 
    public float damage; 
    public int HP = 3; 

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody2D>();  
    }

    // Skeleton State Machine Controller
    void Update()
    {
        if(!takingHit)
        {
            switch(currentAIState)
            {
                case AIStates.PATROL:
                    _WallCheck();
                    _Patrol();
                    _LookForPlayer();
                    break;
                case AIStates.CHASE:
                    _Chase();
                    break;
                case AIStates.ATTACKING:
                    if(!isAttacking) // Stops Attack Spam 
                    {
                        StartCoroutine(_Attack());
                    } 
                    break;  
            }
        }

        _Animate();
    }
    // Patrolling Behaviours 
    void _Patrol()
    {
        if(!wallAhead) // Wall Flag
        {
            if(groundAhead) // Ground Flag 
            {
                m_Rigidbody.AddForce(direction * walkSpeed * Time.deltaTime); // Moves object 
                currentAnimState = AnimStates.WALKING; // Sets the Animation State
            }
            else 
            {
                direction *= -1; 
            }
        } 
        else 
        {
            direction *= -1; 
        }
    }
    // Animates the Object based on Animation Enum
    void _Animate()
    {
        // Hack to normalize the direction
        if(direction.x > 0)
        {
            direction = new Vector2(1, 0); 
        } 
        else if (direction.x < 0)
        {
            direction = new Vector2(-1, 0); 
        }
        transform.localScale = new Vector3(direction.x, transform.localScale.y, transform.localScale.z);
        m_Animator.SetInteger("AnimState", (int)currentAnimState);
    }
    // Checks for walls 
    void _WallCheck()
    {
         var wallHit = Physics2D.Linecast(transform.position, wallSensor.transform.position, collisionWallLayer);
         if(wallHit)
         {
             if(wallHit.collider.CompareTag("Ground"))
             {
                 wallAhead = true; 
             }
         } else {
             wallAhead = false;
         }
    }
    // All Trigger functions are for ground checks
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Ground"))
        {
            groundAhead = true; 
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Ground"))
        {
            groundAhead = true; 
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Ground"))
        {
            groundAhead = false; 
        }
    }
    // Circles Casts to look for player 
    void _LookForPlayer()
    {
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        var contacts = Physics2D.CircleCast(transform.position, sightRange, new Vector2(0, 0), filter, results);
        if(contacts > 0)
        {
            playerInSight = true; // Player in Sight flag
            target = results[0].collider.gameObject; 
            currentAIState = AIStates.CHASE;
        } 
        else 
        {
            playerInSight = false;
            target = null; 
            currentAIState = AIStates.PATROL; 
        }
    }
    // Chase Behaviour 
    void _Chase()
    {
        if(target)
        {
            direction = Vector3.Normalize(target.transform.position - transform.position); // Normalize direction to Player
            direction = new Vector2(direction.x, 0); // remove Y
            m_Rigidbody.AddForce(direction * chaseSpeed * Time.deltaTime);
            if(Vector2.Distance(target.transform.position, transform.position) < attackRange)
            {
                currentAIState = AIStates.ATTACKING; // Change to Attacking State if in Range
            } 
            else 
            {
                _LookForPlayer(); // Check if Player is still in range
            }
        }
    }

    void _Idle()
    {
        currentAnimState = AnimStates.IDLE;
    }
    // Attacking Coroutine
    IEnumerator _Attack()
    {
        isAttacking = true; // Stops Attack spam
        currentAnimState = AnimStates.ATTACK; // Changed Animation
        yield return new WaitForSeconds(0.667f); // Wait for animation 
        // Check for Damage on Player
        CheckForDamage();
        _Idle(); // Pause after attacking
        yield return new WaitForSeconds(1.0f);
        _LookForPlayer(); // Check if player is still in range
        isAttacking = false;  // reset attacking flag
    }
    // Circle Cast at around object to look for player and damage them
    void CheckForDamage()
    {
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        var hit = Physics2D.CircleCast(transform.position, attackRange, new Vector2(0, 0), filter, results);
        if(hit > 0)
        {
            hitPlayer = true;
            if(target.TryGetComponent<PlayerController>(out PlayerController controller))
            {
                SoundManager.instance.PLaySE("Sword1");
                controller.TakeDamage(damage, direction);
            }
        } else {
            hitPlayer = false;
        }
    }

    public void TakeHit(Vector2 direction)
    {
        if(!takingHit)
        {
            HP--;
            m_Rigidbody.AddForce(direction * 150); // Push back after hit
            currentAnimState = AnimStates.HIT;
            StopAllCoroutines();
            StartCoroutine(OnHit());
            if(HP <= 0)
            {
                StopAllCoroutines();
                StartCoroutine(OnDeath());
            } 
        }
        
    }

    IEnumerator OnHit()
    {
        takingHit = true; 
        yield return new WaitForSeconds(1.00f);
        takingHit = false;
        isAttacking = false;  
        currentAIState = AIStates.PATROL;
    }

    IEnumerator OnDeath()
    {
        m_Animator.SetTrigger("Death");
        FindObjectOfType<PlayerController>().AddToScore(50);
        yield return new WaitForSeconds(1.50f);
        Destroy(gameObject); 
    }
}
