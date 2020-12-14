/**
    VanishingPlatform.cs
    Nabil Babu
    101214336
    Dec 12th 2020
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingPlatform : MonoBehaviour
{
    public Animator animator;
    public float vanishTimer;  
    public float delayTimer;  
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if(controller)
            {
                StartCoroutine(Vanish());
            }
        }
        
    }

    IEnumerator Vanish()
    {
        yield return new WaitForSeconds(delayTimer);
        animator.SetTrigger("FadeAway");
        yield return new WaitForSeconds(1.0f);
        gameObject.layer = LayerMask.NameToLayer("Vanish");  
        yield return new WaitForSeconds(vanishTimer);
        gameObject.layer = LayerMask.NameToLayer("Default");  
        animator.SetTrigger("Idle"); 
    }
}
