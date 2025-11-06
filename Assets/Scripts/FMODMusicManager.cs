using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class FMODMusicManager : MonoBehaviour
{
    [Header("Evento FMOD de música")]
    [SerializeField] private EventReference musicaNivelesEvent;

    [Header("Parámetro de FMOD")]
    [SerializeField] private string variable = "NIVEL";

    private EventInstance musicaInstance;
    private bool isPaused = false;

    [Header("Nivel actual de parámetro")]
    public int nivelActual = 1;

    private static FMODMusicManager instance;

    private void Awake()
    {
        // ✅ Asegura que solo exista uno entre escenas
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        musicaInstance = RuntimeManager.CreateInstance(musicaNivelesEvent);
        musicaInstance.start();

        // Inicializa el parámetro si hace falta
        SetNivel(nivelActual);
    }

    // ==============================
    // ======= CONTROL DE NIVEL =====
    // ==============================

    public void SetNivel(int valor)
    {
        nivelActual = valor;
        musicaInstance.setParameterByName(variable, nivelActual);
        Debug.Log($"[FMOD] Parámetro {variable} cambiado a {nivelActual}");
    }

    public void AvanzarNivel()
    {
        nivelActual++;
        SetNivel(nivelActual);
    }

    // ==============================
    // ======= CONTROL DE ESCENA =====
    // ==============================

    public void AvanzarEscena()
    {
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        int totalEscenas = SceneManager.sceneCountInBuildSettings;

        if (escenaActual + 1 < totalEscenas)
        {
            Debug.Log($"[FMOD] Avanzando de escena {escenaActual} → {escenaActual + 1}");
            SceneManager.LoadScene(escenaActual + 1);

            // 👇 Opcional: Avanza también el parámetro FMOD al cambiar escena
            AvanzarNivel();
        }
        else
        {
            Debug.Log("[FMOD] No hay más escenas en el Build Settings.");
        }
    }

    // ==============================
    // ======= PAUSA Y RESUME =======
    // ==============================

    public void PauseMusic()
    {
        if (isPaused) return;
        isPaused = true;
        musicaInstance.setPaused(true);
        Debug.Log("[FMOD] Música pausada");
    }

    public void ResumeMusic()
    {
        if (!isPaused) return;
        isPaused = false;
        musicaInstance.setPaused(false);
        Debug.Log("[FMOD] Música reanudada");
    }

    private void OnDestroy()
    {
        musicaInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicaInstance.release();
    }
}
