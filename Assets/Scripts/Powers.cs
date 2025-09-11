using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers : MonoBehaviour
{    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Animator playerAnimator = collision.GetComponent<Animator>();
            if (!playerAnimator.GetBool("Halloween"))
            {
                Debug.Log("toco la calabaza");
                playerAnimator.SetBool("Halloween", true);
                playerAnimator.SetBool("Inicio", false);
                playerAnimator.Play("Idle");
            }
            else
            {
                Debug.Log("toco el gorro");
                playerAnimator.SetBool("Navidad", true); 
                playerAnimator.Play("Idle 0"); 
            }
        }

    }
}
