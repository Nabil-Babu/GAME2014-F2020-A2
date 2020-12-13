using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed; 
    public float pauseTimer; 
    public bool atStartPoint = true;
    public bool atEndPoint = false;
    public bool lockPosition = false;
    public GameObject startPoint; 
    public GameObject endPoint;
    [SerializeField]
    private Vector2 direction;
    [SerializeField]
    private Rigidbody2D m_rigidbody2D;   

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        transform.position = startPoint.transform.position;
        // Get the Initial direction from start to end
        direction = Vector3.Normalize(endPoint.transform.position - startPoint.transform.position);
    }

    // At Current State Start and End points cannot be Dynamically changed at runtime
    void Update()
    {
        if(!lockPosition)
        {
            if(atStartPoint)
            {
                // Translate based on direction and speed set at Start()
                transform.Translate(direction * speed * Time.deltaTime);
                // Check current distance to the target, 0.10 is arbitrary threshold accounts for floating point errors
                if(Vector2.Distance(transform.position, endPoint.transform.position) < 0.20)
                {
                    // Set platforms positions
                    transform.position = endPoint.transform.position;
                    //Flips flags for platforms current state 
                    atStartPoint = false;
                    atEndPoint = true;
                    // Start Pause Timer
                    StartCoroutine(PauseTimer());
                } 
            } 
            else if (atEndPoint)
            {
                // Translate in the opposite direction 
                transform.Translate(-direction * speed*Time.deltaTime);
                // Check current distance to the target 
                if(Vector2.Distance(transform.position, startPoint.transform.position) < 0.20)
                {
                    // Set platforms positions
                    transform.position = startPoint.transform.position;
                    //Flips flags for platforms current state 
                    atStartPoint = true;
                    atEndPoint = false;
                    // Start Pause Timer
                    StartCoroutine(PauseTimer());
                } 
            }    
        }
    }

    IEnumerator PauseTimer()
    {
        lockPosition = true;
        yield return new WaitForSeconds(pauseTimer);
        lockPosition = false;
    }
}
