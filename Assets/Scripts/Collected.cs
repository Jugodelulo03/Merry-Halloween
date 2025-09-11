using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Collected : MonoBehaviour
{
    public AudioSource sonido;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GetComponent<SpriteRenderer>().enabled = false;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            sonido.Play();
            //FindObjectOfType<FruitManager>().AllFruitsCollected();
            Destroy(gameObject,0.5f);
        }
    }


}
