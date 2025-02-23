using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : IOccupier
{
    public Transform OccupierTransform { get; set; }
}

public class Board : MonoBehaviour
{
    private List<Tile> tiles = new();
    public static event Action<Board> OnBoardReady;
    
    public Tile CenterTile { get; set; }

    [SerializeField] private bool boardDebug;
    
    private void Start()
    {
        OnBoardReady?.Invoke(this);
    }

    public void AddTile(Tile tile)
    {
        tiles.Add(tile);
    }

    public List<Tile> GetTiles()
    {
        return tiles;
    }

    private void Update()
    {
        if (!boardDebug)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowRange(CenterTile, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowRange(CenterTile, 2);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ShowRange(CenterTile, 3);
        }
    }

    // get centre tile.
    // 1. This can be used to centre the camera and size of play area can push back the camera on the Z axis.
    

    // Get the tile that the player is on for AI Pathfinding.
    public Tile GetPlayerTile()
    {
        foreach (Tile tileInstance in tiles)
        {
            // todo change PlayerTest to the actual script on the player.
            if (tileInstance.Occupier is Player)
            {
                return tileInstance;
            }
        }
        return null;
    }

    private void ShowRange(Tile fromTile, int range)
    {
        // fills out the range to allow for one step diagonally.
        float adjustedRange = range + 0.42f;
        foreach (var tile in tiles)
        {
            // disables any leftover range indicators that may erroneously enabled from previous turn.
            tile.TileRangeIndicator.SetActive(false);
            
            float distanceToTile = Vector3.Distance(tile.transform.position, fromTile.transform.position);
            if (distanceToTile <= adjustedRange)
            {
                tile.TileRangeIndicator.SetActive(true);
            }
        }
    }
}
