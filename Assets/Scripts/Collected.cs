using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Collected : MonoBehaviour
{
    [Header("Evento de sonido en FMOD")]
    [Tooltip("Arrastra aquí el evento de sonido desde el Event Browser de FMOD")]
    public EventReference sonidoFMOD;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            // Desactivar sprite del recolectable
            GetComponent<SpriteRenderer>().enabled = false;

            // Activar efecto visual hijo (por ejemplo, partículas o brillo)
            if(transform.childCount > 0)
                gameObject.transform.GetChild(0).gameObject.SetActive(true);

            // Reproducir evento de sonido FMOD en la posición del objeto
            RuntimeManager.PlayOneShot(sonidoFMOD, transform.position);

            // Destruir el objeto después de un pequeño retraso
            Destroy(gameObject, 0.5f);
        }
    }
}
