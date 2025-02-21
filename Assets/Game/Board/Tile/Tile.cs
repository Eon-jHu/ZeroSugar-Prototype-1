using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector] public List<Tile> neighbourTiles = new();
    [field: SerializeField] public GameObject TileRangeIndicator { get; private set; }

    [field: SerializeField]
    public IOccupier Occupier { get; private set; }
    private Transform occupierTransform;

    public static event Action<Tile> OnCenterTileAssigned;

    private void Awake()
    {
        Board.OnBoardReady += InitializeTile;
    }

    public bool OccupyTile(IOccupier occupier, Tile fromTile, bool snapToTile = false)
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
        occupierTransform = occupier.OccupierTransform;

        /* Note that this probably works best when the player mesh/model is a separate from the Player root object. Then you can ensure
         That the feet of the player model is at 0,0,0.*/
        if (snapToTile)
        {
            Occupier.OccupierTransform.position = transform.position;
        }
        
        return true;
    }

    private void LeaveTile()
    {
        Occupier = null;
        occupierTransform = null;
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
            OnCenterTileAssigned?.Invoke(this);
            transform.name = "Center Tile";
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
