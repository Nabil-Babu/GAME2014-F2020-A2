/**
    DeathBox.cs
    Nabil Babu
    101214336
    Dec 12th 2020
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
   void OnTriggerEnter2D(Collider2D other)
   {
       if(other.TryGetComponent<PlayerController>(out PlayerController controller))
       {
           controller.OnDeath();
       }
   }
}
