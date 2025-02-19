using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private List<Tile> neighbourTiles = new();
    [field: SerializeField] public GameObject TileRangeIndicator { get; private set; }
    public IOccupier Occupier { get; private set; }

    public static event Action OnCenterTileAssigned;

    private void Awake()
    {
        Board.OnBoardReady += InitializeTile;
    }

    public bool OccupyTile(IOccupier occupier, Tile fromTile)
    {
        // if tile is already occupied return false.
        // todo check for unit death--Occupier might not be null
        if (Occupier != null)
            return false;
        
        // leave current tile so that it can be occupied by another unit.
        if (fromTile)
        {
            fromTile.LeaveTile();
        }
        Occupier = occupier;
        return true;
    }

    private void LeaveTile()
    {
        Occupier = null;
    }

    private void InitializeTile(Board board)
    {
        foreach (var tile in FindObjectsOfType<Tile>())
        {
            if (tile == this)
                continue;
            
            float distanceToOtherTile = Vector3.Distance(transform.position, tile.transform.position);

            if (distanceToOtherTile <= 1.42f)
            {
                neighbourTiles.Add(tile);
            }
        }
        
        // register tile with the board
        board.AddTile(this);

        // if the tile is at origin, this is the center tile.
        if (transform.position == Vector3.zero)
        {
            board.CenterTile = this;
            OnCenterTileAssigned?.Invoke();
        }
    }

    // can use this for enemy spawn positions
    public bool IsBoundaryTile()
    {
        return neighbourTiles.Count < 8;
    }

    public bool IsOccupied()
    {
        return Occupier != null;
    }

    private void OnDestroy()
    {
        Board.OnBoardReady -= InitializeTile;
    }
}
