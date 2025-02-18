using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<Tile> neighbourTiles = new();
    public IOccupier Occupier { get; private set; }

    private void Awake()
    {
        Board.OnBoardReady += GetNeighbourTiles;
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

    private void GetNeighbourTiles()
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
    }

    // can use this for enemy spawn positions
    private bool IsBoundaryTile()
    {
        return neighbourTiles.Count < 8;
    }

    private void OnDestroy()
    {
        Board.OnBoardReady -= GetNeighbourTiles;
    }
}
