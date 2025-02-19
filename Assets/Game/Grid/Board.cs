using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : IOccupier {}

public class Board : MonoBehaviour
{
    private List<Tile> tiles = new();
    public static event Action OnBoardReady;
    
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
