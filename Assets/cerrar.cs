using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cerrar : MonoBehaviour
{
    // Start is called before the first frame update
    public void CerrarJuego()
    {
        // Cierra la aplicaci�n
        Application.Quit();

        // Si est�s en el editor, detiene la ejecuci�n del juego
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
