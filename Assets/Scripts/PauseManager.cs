using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class PauseManager : MonoBehaviour
{
    // ===== SINGLETON =====
    public static PauseManager Instance { get; private set; }
    public static bool InstanceExists => Instance != null;

    // ===== ESTADO DEL JUEGO =====
    public static bool GameIsPaused = false;

    [Header("UI del menú de pausa")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("FMOD Events")]
    [SerializeField] private EventReference musicaNivelesEvent;
    [SerializeField] private EventReference musicaMenuPausaEvent;
    [SerializeField] private EventReference cerrarMenuPausaEvent;

    private EventInstance musicaNivelesInstance;
    private EventInstance musicaMenuInstance;

    private bool musicaGameplayActiva = false;
    private bool musicaMenuActiva = false;
    private int valorNivel = 1;

    [Header("Escena donde se destruye automáticamente")]
    [SerializeField] private string escenaDestruir = "MenuPrincipal";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Crear e iniciar la música del gameplay
        musicaNivelesInstance = RuntimeManager.CreateInstance(musicaNivelesEvent);
        musicaNivelesInstance.start();
        musicaGameplayActiva = true;

        // Inicializa parámetro global NIVEL
        RuntimeManager.StudioSystem.setParameterByName("NIVEL", valorNivel);
    }

    void Update()
    {
        // -----------------------------------------
        // SISTEMA DE INPUT (TECLADO + GAMEPAD)
        // -----------------------------------------
        bool pausePressed = false;

        // Keyboard / Cancel button
        if (Input.GetButtonDown("Cancel"))
            pausePressed = true;

        // GAMEPADS Start/Options (muy común)
        if (Input.GetKeyDown(KeyCode.JoystickButton7)) // Start en Xbox/PlayStation
            pausePressed = true;

        // Si se presionó pausa
        if (pausePressed)
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    // ==============================
    // ======= MÉTODOS DE PAUSA =====
    // ==============================
    public void Pause()
    {
        if (GameIsPaused || musicaMenuActiva)
            return;

        GameIsPaused = true;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        // Pausar música gameplay
        if (musicaGameplayActiva)
            musicaNivelesInstance.setPaused(true);

        // Crear música de menú solo si no existe
        if (!musicaMenuActiva)
        {
            musicaMenuInstance = RuntimeManager.CreateInstance(musicaMenuPausaEvent);
            musicaMenuInstance.start();
            musicaMenuActiva = true;
        }
    }

    public void Resume()
    {
        if (!GameIsPaused)
            return;

        GameIsPaused = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Detener música menú
        if (musicaMenuActiva)
        {
            musicaMenuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicaMenuInstance.release();
            musicaMenuActiva = false;
        }

        // Reanudar música gameplay
        if (musicaGameplayActiva)
            musicaNivelesInstance.setPaused(false);

        // Sonido de cierre
        RuntimeManager.PlayOneShot(cerrarMenuPausaEvent, Vector3.zero);
    }

    // ==============================
    // ======= MÉTODOS FMOD =========
    // ==============================
    public void IncrementarNivelFMOD()
    {
        valorNivel += 2;
        if (valorNivel > 9)
            valorNivel = 9;

        RuntimeManager.StudioSystem.setParameterByName("NIVEL", valorNivel);
        Debug.Log($"[FMOD] Escena cambiada: parámetro NIVEL = {valorNivel}");
    }

    public void SetNivelFMOD(int nuevoValor)
    {
        valorNivel = Mathf.Clamp(nuevoValor, 1, 9);
        RuntimeManager.StudioSystem.setParameterByName("NIVEL", valorNivel);
        Debug.Log($"[FMOD] NIVEL establecido manualmente: {valorNivel}");
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    // ==============================
    // ======= AUTO-DESTRUCCIÓN =====
    // ==============================
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == escenaDestruir)
        {
            Debug.Log($"[FMOD] Escena '{scene.name}' detectada — destruyendo PauseManager");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (musicaGameplayActiva)
        {
            musicaNivelesInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicaNivelesInstance.release();
        }

        if (musicaMenuActiva)
        {
            musicaMenuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicaMenuInstance.release();
        }
    }
}
