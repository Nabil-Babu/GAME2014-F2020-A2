  
/**
    CameraFollow.cs
    Nabil Babu
    101214336
    Dec 12th 2020
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player; 

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(player.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
    }
}
