using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryBox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
   {
       if(other.TryGetComponent<PlayerController>(out PlayerController controller))
       {
           controller.OnEndGame();
       }
   }
}
