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
    private Rigidbody2D rigidbody2D;   

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        transform.position = startPoint.transform.position;
        direction = Vector3.Normalize(endPoint.transform.position - startPoint.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(!lockPosition)
        {
            if(atStartPoint)
            {
                transform.Translate(direction * speed * Time.deltaTime);
                if(Vector2.Distance(transform.position, endPoint.transform.position) < 0.10)
                {
                    transform.position = endPoint.transform.position;
                    atStartPoint = false;
                    atEndPoint = true;
                    StartCoroutine(PauseTimer());
                } 
            } 
            else if (atEndPoint)
            {
                transform.Translate(-direction * speed*Time.deltaTime);
                if(Vector2.Distance(transform.position, startPoint.transform.position) < 0.10)
                {
                    transform.position = startPoint.transform.position;
                    atStartPoint = true;
                    atEndPoint = false;
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
