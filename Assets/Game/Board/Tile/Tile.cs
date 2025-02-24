using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<Tile> neighbourTiles = new();
    [field: SerializeField] public GameObject TileRangeIndicator { get; private set; }

    [field: SerializeField]
    public IOccupier Occupier { get; private set; }
    private Transform occupierTransform;
    private Color previousColor;

    [SerializeField] private Color hoverGreen;
    [SerializeField] private Color hoverYellow;
    [SerializeField] private Color hoverRed;

    public static event Action<Tile> OnCenterTileAssigned;

    private void Awake()
    {
        Board.OnTileInitRequired += InitializeTile; 
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

            if (distanceToOtherTile <= 1.42f * Board.Instance.TileSize)
            {
                neighbourTiles.Add(tile);
            }
        }
        
        // register tile with the board
        board.AddTile(this);

        // if the tile is at origin, this is the center tile.
        if (transform.name.ToLower().Contains("center tile"))
        {
            board.CenterTile = this;
            OnCenterTileAssigned?.Invoke(this);
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

    public void ShowTileIndicator(bool isThrownRange = false)
    {
        TileRangeIndicator.SetActive(true);
        Color color = isThrownRange ? hoverYellow : hoverGreen;
        TileRangeIndicator.GetComponent<Renderer>().material.color = color;
        previousColor = color;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == GetComponent<Collider>())
            {
                SetHoverColor();
            }
        }
    }
    
    private void OnMouseEnter()
    {
        if (!TileRangeIndicator.activeSelf)
            return;

        SetHoverColor();
    }

    private void SetHoverColor()
    {
        Renderer render = TileRangeIndicator.GetComponent<Renderer>();
        previousColor = render.material.color;
        render.material.color = hoverRed;
    }
    
    private void OnMouseExit()
    {
        if (!TileRangeIndicator.activeSelf)
            return;

        Renderer render = TileRangeIndicator.GetComponent<Renderer>();
        render.material.color = previousColor;
    }


    private void OnDestroy()
    {
        Board.OnTileInitRequired -= InitializeTile; 
    }
}
