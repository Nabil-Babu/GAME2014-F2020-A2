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
                    if(!isAttacking)
                    {
                        StartCoroutine(_Attack());
                    } 
                    break;  
            }
        }

        _Animate();
    }

    void _Patrol()
    {
        if(!wallAhead)
        {
            if(groundAhead)
            {
                m_Rigidbody.AddForce(direction * walkSpeed * Time.deltaTime);
                currentAnimState = AnimStates.WALKING;
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

    void _Animate()
    {
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

    void _LookForPlayer()
    {
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        var contacts = Physics2D.CircleCast(transform.position, sightRange, new Vector2(0, 0), filter, results);
        if(contacts > 0)
        {
            playerInSight = true;
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

    void _Chase()
    {
        if(target)
        {
            direction = Vector3.Normalize(target.transform.position - transform.position);
            direction = new Vector2(direction.x, 0);
            m_Rigidbody.AddForce(direction * chaseSpeed * Time.deltaTime);
            if(Vector2.Distance(target.transform.position, transform.position) < attackRange)
            {
                currentAIState = AIStates.ATTACKING;
            } 
            else 
            {
                _LookForPlayer();
            }
        }
    }

    void _Idle()
    {
        currentAnimState = AnimStates.IDLE;
    }

    IEnumerator _Attack()
    {
        isAttacking = true; 
        currentAnimState = AnimStates.ATTACK; 
        yield return new WaitForSeconds(0.667f);
        // Check for Damage on Player
        CheckForDamage();
        _Idle();
        yield return new WaitForSeconds(1.0f);
        _LookForPlayer(); 
        isAttacking = false;  
    }

    void CheckForDamage()
    {
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        var hit = Physics2D.CircleCast(transform.position, attackRange, new Vector2(0, 0), filter, results);
        if(hit > 0)
        {
            hitPlayer = true;
            if(target.TryGetComponent<PlayerController>(out PlayerController controller))
            {
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
            m_Rigidbody.AddForce(direction * 150);
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
