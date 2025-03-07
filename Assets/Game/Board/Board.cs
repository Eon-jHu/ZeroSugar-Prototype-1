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
            if (tileInstance.Occupier is Player)
            {
                return tileInstance;
            }
        }
        return null;
    }
    
    public void ShowRange(Tile currentTile, int minRange, int maxRange, Card card)
    {
        // Fill out the range, and unfill all tiles closer than the minimum range.
        foreach (var tile in tiles)
        {
            if (TileIsInOptimalRange(currentTile, tile, minRange, maxRange, card))
            {
                tile.ShowTileIndicator();
            }
            else
            {
                tile.ShowTileIndicator(true);
            }
        }
    }
    
    public bool TileIsInOptimalRange(Tile currentTile, Tile targetTile, int minRange, int maxRange, Card card)
    {
        if (card.cardData.aoeType == AoEType.Line)
        {
            return GetLineTiles(currentTile, maxRange).Contains(targetTile);
        }
        
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

    //todo: investigate why this function is no longer used.
    public void HandleOccupierDeath(IOccupier occupier)
    {
        Tile tile = GetOccupierTile(occupier);

        if (tile)
        {
            tile.LeaveTile(occupier);
        }
    }
    
    public Tile GetTileClosestToPosition(Vector3 position)
    {
        float closestDistance = float.MaxValue;
        Tile closestTile = null;
        foreach (var tile in tiles)
        {
            float distance = Vector3.Distance(tile.transform.position, position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }

        return closestTile;
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
            Debug.Log(TileSize);
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

    public Tile GetEnemyTile(Enemy enemy)
    {
        Tile enemyTile = null;

        foreach (var tile in tiles)
        {
            if (tile.Occupier == enemy as IOccupier)
            {
                enemyTile = tile;
            }
        }
        return enemyTile;
    }

    public List<Tile> GetLineTiles(Tile currentTile, int range) 
    {
        List<Tile> lineTiles = new();

        // find all the diagonal tiles to begin with
        foreach (var tile in currentTile.neighbourTiles)
        {
            // add all the adjacent tiles.
            lineTiles.Add(tile);
            Vector3 direction = tile.transform.position - currentTile.transform.position;
            
            // go out in the direction 
            for (int i = 1; i < range; i++)
            {
                // if the direction magnitude is greater than 1, it'll be diagonal
                Vector3 samplePos = currentTile.transform.position + direction * i  * TileSize;
                Tile closestTile = GetTileClosestToPosition(samplePos);
                lineTiles.Add(closestTile);
            }
        }
        return lineTiles;
    }
}
