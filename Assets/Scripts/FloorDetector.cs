using UnityEngine;
using UnityEngine.Tilemaps;
using FMODUnity;
using FMOD.Studio;

public class FloorDetector : MonoBehaviour
{
    [Header("Referencias")]
    public Tilemap tilemap;
    public BoxCollider2D checkGround;
    private Rigidbody2D rb; // para saber si el jugador se mueve

    [Header("Tipos de Tile")]
    public TileBase[] piedraTiles;
    public TileBase[] hieloTiles;
    public TileBase[] cristalTiles;

    [Header("Evento FMOD de caminar con parámetro Floor")]
    [SerializeField] private EventReference footstepsEvent;

    [Header("Footstep Timing")]
    public float stepInterval = 0.4f; // tiempo entre pasos
    private float stepTimer;

    [Header("Debug")]
    public string currentTileName;
    public string currentTileTipo;

    private TileBase ultimoTile;
    private EventInstance currentFootstep;
    private float currentFloorValue = 0f; // parámetro en FMOD (0 = piedra, 1 = hielo, 2 = cristal)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (CheckGround.isGrounded)
        {
            DetectarTileDesdeCollider();

            // Reproducir pasos mientras se mueve
            if (rb.velocity.magnitude > 0.1f) // si se está moviendo
            {
                stepTimer -= Time.deltaTime;
                if (stepTimer <= 0f)
                {
                    ReproducirSonido(currentFloorValue);
                    stepTimer = stepInterval;
                }
            }
            else
            {
                stepTimer = 0f; // resetea si está quieto
            }
        }
        else
        {
            currentTileName = "En el aire";
            currentTileTipo = "Ninguno";
        }
    }

    private void DetectarTileDesdeCollider()
    {
        Vector3 posicion = checkGround.bounds.center;
        Vector3Int celda = tilemap.WorldToCell(posicion);

        TileBase tile = tilemap.GetTile(celda);

        if (tile != null)
        {
            currentTileName = tile.name;

            // Guardar tipo de suelo (aunque no cambie el tile)
            ClasificarTile(tile);
        }
        else
        {
            currentTileName = "Ninguno";
            currentTileTipo = "Ninguno";
        }
    }

    private void ClasificarTile(TileBase tile)
    {
        string tileName = tile.name;

        if (System.Array.Exists(piedraTiles, t => t != null && t.name == tileName))
        {
            currentTileTipo = "Piedra";
            currentFloorValue = 0f;
        }
        else if (System.Array.Exists(hieloTiles, t => t != null && t.name == tileName))
        {
            currentTileTipo = "Hielo";
            currentFloorValue = 1f;
        }
        else if (System.Array.Exists(cristalTiles, t => t != null && t.name == tileName))
        {
            currentTileTipo = "Cristal";
            currentFloorValue = 2f;
        }
        else
        {
            currentTileTipo = "Otro";
        }

        Debug.Log($"Tile detectado: {currentTileName} → Tipo: {currentTileTipo}");
    }

    private void ReproducirSonido(float floorValue)
    {
        if (currentFootstep.isValid())
        {
            currentFootstep.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentFootstep.release();
        }

        currentFootstep = RuntimeManager.CreateInstance(footstepsEvent);

        if (currentFootstep.isValid())
        {
            currentFootstep.setParameterByName("Floor", floorValue);
            currentFootstep.start();
            currentFootstep.release();
        }
    }
}
