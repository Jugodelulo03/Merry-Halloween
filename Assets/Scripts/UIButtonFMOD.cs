using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

[RequireComponent(typeof(Button))]
public class UIButtonFMOD : MonoBehaviour
{
    [Header("Evento de FMOD para este botón")]
    [SerializeField] private EventReference botonEvent;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ReproducirSonidoFMOD);
    }

    private void ReproducirSonidoFMOD()
{

    if (botonEvent.IsNull)
    {
        Debug.LogWarning($"[FMOD] No se asignó un evento FMOD al botón {gameObject.name}");
        return;
    }

    RuntimeManager.PlayOneShot(botonEvent, Vector3.zero);
}

    void OnDestroy()
    {
        // Evita memory leaks
        button.onClick.RemoveListener(ReproducirSonidoFMOD);
    }
}
