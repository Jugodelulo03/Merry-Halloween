using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity; // Importante para usar FMOD

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private EventReference checkpointSound; // Asigna el evento FMOD desde el inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerRespawn>().ReachedCheckpoint(transform.position.x, transform.position.y);
            GetComponent<Animator>().enabled = true;

            // Reproducir sonido FMOD
            RuntimeManager.PlayOneShot(checkpointSound, transform.position);
        }
    }
}

