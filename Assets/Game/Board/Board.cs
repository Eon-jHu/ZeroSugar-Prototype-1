using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : Singleton<Board>
{
    public static readonly string SizeIndicatorString = "SizeIndicator";

    private List<Tile> tiles = new();
    
    public int TileSize { get; set; }

    public static event Action<Board> OnTileInitRequired;
    public static event Action<Board> OnBoardReady;
    
    public Tile CenterTile { get; set; }

    [SerializeField] private bool boardDebug;
    
    private void Start()
    {
        /* dirty way to see the size of the tile board; Carry data over from editor scripts is actually a bit annoying
         so I just created empty name GameObjects to tell the size of the tiles since getting the scale value of the tile works for now but may change. */
        foreach (var go in transform.GetComponentsInChildren<Transform>())
        {
            if (go.transform.name.Contains(SizeIndicatorString))
            {
                TileSize++;
            }
        }

        if (TileSize <= 0)
        {
            Debug.LogError("Tile size is 0. Please recreate the board from tools>board spawner.");
        }
        
        OnTileInitRequired?.Invoke(this);
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

    public void ShowRange(Tile currentTile, int minRange, int maxRange)
    {
        // Fill out the range, and unfill all tiles closer than the minimum range.
        foreach (var tile in tiles)
        {
            if (TileIsInOptimalRange(currentTile, tile, minRange, maxRange))
            {
                tile.ShowTileIndicator();
            }
            else
            {
                tile.ShowTileIndicator(true);
            }
        }
    }

    public bool TileIsInOptimalRange(Tile currentTile, Tile targetTile, int minRange, int maxRange)
    {
        float adjustedMaxRange = maxRange + 0.421f;
        float adjustedMinRange = minRange + 0.419f;
        
        float distanceToTile = GetDistance(currentTile, targetTile);
        
        return (distanceToTile - 0.01f <= adjustedMaxRange * TileSize && distanceToTile + 0.01f >= adjustedMinRange * TileSize);
    }

    public void DisableShowRange()
    {
        foreach (var tile in tiles)
        {
            if (tile.isInFeedbackMode) continue;

            tile.TileRangeIndicator.SetActive(false);
        }
    }

    public float GetDistance(Tile tile, Tile targetTile)
    {
        return Vector3.Distance(tile.transform.position, targetTile.transform.position) - 0.42f;
    }

    public Tile GetOccupierTile(IOccupier occupier)
    {
        foreach (var tile in tiles)
        {
            if (tile.Occupier == occupier)
            {
                return tile;
            }
        }
        return null;
    }

    public void HandleOccupierDeath(IOccupier occupier)
    {
        Tile tile = GetOccupierTile(occupier);

        if (tile)
        {
            tile.LeaveTile(occupier);
        }
    }

    public void ShowRangeFromCenter(int range)
    {
        ShowRange(CenterTile, range, range);
    }

    public Tile GetNextTileOnPathToPlayer(Tile currentTile, bool diagonalMovement = false)
    {
        Tile playerTile = GetPlayerTile();

        if (playerTile == null)
            return null;

        float closestDistance = float.MaxValue;
        Tile closestTile = null;

        foreach (var tile in currentTile.neighbourTiles)
        {
            float distanceToPlayer = Vector3.Distance(tile.transform.position, playerTile.transform.position);
            
            // disable diagonal movement, make max distance for neighbour to 1 unit space rather than 1.41 unit space that a diagonal uses.
            if (!diagonalMovement && Vector3.Distance(currentTile.transform.position, tile.transform.position) > TileSize * 1.05 
                || tile.IsOccupied())
                continue;

            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                closestTile = tile;
            }
        }
        return closestTile;
    }

    public Tile GetKnockBackTile(Tile currentTile, bool diagonalMovement = false)
    {
        Tile playerTile = GetPlayerTile();

        if (playerTile == null)
            return null;

        float currentDistance = Vector3.Distance(currentTile.transform.position, playerTile.transform.position);
        float farthestDistance = currentDistance; // Start with the current distance
        Tile knockBackTile = null;

        foreach (var tile in currentTile.neighbourTiles)
        {
            float distanceToPlayer = Vector3.Distance(tile.transform.position, playerTile.transform.position);

            // disable diagonal movement, make max distance for neighbour to 1 unit space rather than 1.41 unit space that a diagonal uses.
            if (!diagonalMovement && Vector3.Distance(currentTile.transform.position, tile.transform.position) > TileSize * 1.05
                || tile.IsOccupied())
                continue;

            if (distanceToPlayer > farthestDistance)
            {
                farthestDistance = distanceToPlayer;
                knockBackTile = tile;
            }


        }

        return knockBackTile;
    }


}
