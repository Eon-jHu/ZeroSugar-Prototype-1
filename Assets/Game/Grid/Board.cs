using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : IOccupier {}

public class Board : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private GameObject tile;

    private List<Tile> tiles = new();

    public static event Action OnBoardReady;

    private void Awake()
    {
        float offsetX = (width - 1) / 2f;
        float offsetZ = (height - 1) / 2f;
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(j - offsetX, 0, i - offsetZ), Quaternion.identity);
                newTile.name = "Tile " + i + "_" + j;
                newTile.transform.parent = transform;
            }
        }
    }
    
    private void Start()
    {
        OnBoardReady?.Invoke();
    }
    
    // get centre tile.
    // 1. This can be used to centre the camera and size of play area can push back the camera on the Z axis.
    
    // Get the tile that the player is on for AI Pathfinding.
    public Tile GetPlayerTile()
    {
        foreach (Tile tileInstance in tiles)
        {
            // todo change PlayerTest to the actual script on the player.
            if (tileInstance.Occupier is PlayerTest)
            {
                return tileInstance;
            }
        }
        return null;
    }

}
