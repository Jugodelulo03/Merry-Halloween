using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("UI del menú de pausa")]
    [SerializeField] private GameObject pauseMenuUI;

    void Start()
    {
        // Aseguramos que el menú esté oculto al iniciar
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        GameIsPaused = true;
        Time.timeScale = 0f; // Detiene el juego

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
    }

    public void Resume()
    {
        GameIsPaused = false;
        Time.timeScale = 1f; // Reanuda el juego

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
