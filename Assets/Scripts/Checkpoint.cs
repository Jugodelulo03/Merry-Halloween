using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Checkpoint : MonoBehaviour
{
    public AudioSource check;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerRespawn>().ReachedCheckpoint(transform.position.x, transform.position.y);
            GetComponent<Animator>().enabled = true;
            check.Play();
        }

    }
}
