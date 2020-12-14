/**
    VictoryBox.cs
    Nabil Babu
    101214336
    Dec 13th 2020
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryBox : MonoBehaviour
{
    // Triggers the End Title
    void OnTriggerEnter2D(Collider2D other)
   {
       if(other.TryGetComponent<PlayerController>(out PlayerController controller))
       {
           controller.OnEndGame();
       }
   }
}
