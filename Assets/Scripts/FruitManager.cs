using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FruitManager : MonoBehaviour
{
    public GameObject transition;

    private bool sceneChanging = false; // Evita múltiples llamadas

    void Update()
    {
        AllFruitsCollected();
    }

    public void AllFruitsCollected()
    {
        if (transform.childCount == 0 && !sceneChanging)
        {
            sceneChanging = true;

            // Activar transición visual
            if (transition != null)
                transition.SetActive(true);

            // 🔊 Actualizar variable FMOD antes de cambiar de escena
            if (PauseManager.InstanceExists)
                PauseManager.Instance.IncrementarNivelFMOD();

            // Cambiar de escena después de una breve pausa
            Invoke(nameof(ChangeScene), 1f);
        }
    }

    public void ChangeScene()
    {
        PlayerPrefs.DeleteKey("checkPointPositionX");
        PlayerPrefs.DeleteKey("checkPointPositionY");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
