using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cerrar : MonoBehaviour
{
    // Start is called before the first frame update
    public void CerrarJuego()
    {
        // Cierra la aplicación
        Application.Quit();

        // Si estás en el editor, detiene la ejecución del juego
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
