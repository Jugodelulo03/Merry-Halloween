using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using FMODUnity;
using FMOD.Studio;

public class FootstepManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap groundTilemap;   // Asigna tu Tilemap en el inspector

    [Header("Tiles por superficie")]
    public TileBase[] stoneTiles;
    public TileBase[] iceTiles;
    public TileBase[] cristalTiles;

    [Header("FMOD")]
    [EventRef] public string footstepsEvent; // Asigna el evento "Steps"
    private EventInstance footstepsInstance;

    private string currentSurface = "Stone"; // Superficie por defecto

    void Start()
    {
        footstepsInstance = RuntimeManager.CreateInstance(footstepsEvent);
    }

    // 🔹 Llamar esta función cuando el player camine (por ejemplo en animación o input)
    public void PlayFootstep(Vector2 playerPos)
    {
        DetectSurface(playerPos);

        switch (currentSurface)
        {
            case "Stone":
                footstepsInstance.setParameterByName("Floor", 0);
                break;
            case "Ice":
                footstepsInstance.setParameterByName("Floor", 1);
                break;
            case "Cristal":
                footstepsInstance.setParameterByName("Floor", 2);
                break;
        }

        footstepsInstance.start();
    }

    void DetectSurface(Vector2 playerPos)
    {
        Vector3Int tilePos = groundTilemap.WorldToCell(playerPos);
        TileBase tile = groundTilemap.GetTile(tilePos);

        if (tile == null) return;

        if (System.Array.Exists(stoneTiles, t => t == tile))
            currentSurface = "Stone";
        else if (System.Array.Exists(iceTiles, t => t == tile))
            currentSurface = "Ice";
        else if (System.Array.Exists(cristalTiles, t => t == tile))
            currentSurface = "Cristal";
        else
            currentSurface = "Stone"; // Default
    }
}
