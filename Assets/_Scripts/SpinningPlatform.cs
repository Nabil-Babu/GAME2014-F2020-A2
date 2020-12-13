using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningPlatform : MonoBehaviour
{
    public Transform model; 
    public bool lockRotation = false; 
    public bool lockCoroutine = false; 
    public float pauseTimer;
    public float rotationSpeed; 


    // Update is called once per frame
    void Update()
    {
        if(!lockRotation) { model.transform.Rotate(0,0,rotationSpeed); }

        if(Mathf.Abs(model.transform.rotation.eulerAngles.z) % 90.0f <= 1.0f)
        {
            if(!lockCoroutine)
            {
                StartCoroutine(Pause());
            }
            
        } 
    }

    IEnumerator Pause()
    {
        lockRotation = true;
        lockCoroutine = true;
        yield return new WaitForSeconds(pauseTimer);
        lockRotation = false;
        lockCoroutine = false;     
    }
}
